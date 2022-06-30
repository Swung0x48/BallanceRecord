using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using AutoMapper;
using BallanceRecordApi.Contracts.V1;
using BallanceRecordApi.Contracts.V1.Requests;
using BallanceRecordApi.Contracts.V1.Requests.Queries;
using BallanceRecordApi.Contracts.V1.Responses;
using BallanceRecordApi.Domain;
using BallanceRecordApi.Extensions;
using BallanceRecordApi.Helpers;
using BallanceRecordApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BallanceRecordApi.Controllers.V1
{

    public class RoomController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRoomService _roomService;
        private readonly IUriService _uriService;

        public RoomController(IMapper mapper, IRoomService roomService, IUriService uriService)
        {
            _mapper = mapper;
            _roomService = roomService;
            _uriService = uriService;
        }

        [HttpGet(ApiRoutes.Room.GetAll)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationQuery<OrderByEnums.RoomOrderBy> paginationQuery)
        {
            var pagination = _mapper.Map<PaginationFilter<OrderByEnums.RoomOrderBy>>(paginationQuery);
            var rooms = await _roomService.GetRoomsAsync(pagination);
            var roomResponses = _mapper.Map<List<RoomResponse>>(rooms);

            if (pagination is null || pagination.PageNumber < 1 || pagination.PageSize < 1)
            {
                return Ok(new PagedResponse<RoomResponse, OrderByEnums.RoomOrderBy>(roomResponses));
            }
            
            var pagedResponse = PaginationHelpers.CreatePagedResponse(_uriService, pagination, roomResponses);

            return Ok(pagedResponse);
        }
        
        [HttpGet(ApiRoutes.Room.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid roomId)
        {
            var room = await _roomService.GetRoomsByIdAsync(roomId);
            
            if (room is null)
                return NotFound();
            
            return Ok(new Response<RoomResponse>(_mapper.Map<RoomResponse>(room)));
        }
        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost(ApiRoutes.Room.Create)]
        public async Task<IActionResult> Create([FromBody] CreateRoomRequest roomRequest)
        {
            var room = _mapper.Map<Room>(roomRequest);
            room.RoomHostUserId = HttpContext.GetUserId();
            if (room.Id == Guid.Empty)
                room.Id = Guid.NewGuid();

            await _roomService.CreateRoomAsync(room);

            var locationUri = _uriService.GetRoomUri(room.Id.ToString());
            var roomResponse =
                new Response<RoomResponse>(
                    _mapper.Map<RoomResponse>(await _roomService.GetRoomsByIdAsync(room.Id)));
            return Created(locationUri, roomResponse);
        }

        public async Task<bool> UserEligibleToUpdate(Guid roomId)
        {
            var roomHost = await _roomService.GetWhichUserHostsRoom(roomId);
            var userHostsRoom = roomHost is not null && roomHost == HttpContext.GetUserId();
            var userIsAdmin = HttpContext.User.IsInRole("Admin");
            return userHostsRoom || userIsAdmin;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut(ApiRoutes.Room.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid roomId, [FromBody] UpdateRoomRequest request)
        {
            if (!await UserEligibleToUpdate(roomId))
            {
                return BadRequest(new {error = "You do not host this room."});
            }

            var room = await _roomService.GetRoomsByIdAsync(roomId);
            if (room is null)
                return NotFound();

            room.Remark = request.Remark;
            room.Name = request.Name;

            var updated = await _roomService.UpdateRoomAsync(room);
            if (updated)
                return Ok(new Response<RoomResponse>(_mapper.Map<RoomResponse>(room)));
            return NotFound();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPatch(ApiRoutes.Room.StatusTransition)]
        public async Task<IActionResult> ChangeStatus([FromRoute] Guid roomId, [FromBody] RoomStateTransitionRequest request)
        {
            if (!await UserEligibleToUpdate(roomId))
            {
                return BadRequest(new {error = "You do not host this room."});
            }

            var room = await _roomService.GetRoomsByIdAsync(roomId);
            if (room is null)
                return NotFound();
            room.Status = request.Status;
            var updated = await _roomService.UpdateRoomAsync(room);
            if (updated)
                return Ok(new Response<RoomResponse>(_mapper.Map<RoomResponse>(room)));
            return NotFound();
        }
        
        
    }
}