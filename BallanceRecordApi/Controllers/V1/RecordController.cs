using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BallanceRecordApi.Cache;
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
using Newtonsoft.Json;
using Swung0x48.Ballance.TdbReader;

namespace BallanceRecordApi.Controllers.V1
{
    public class RecordController: Controller
    {
        private readonly IRecordService _recordService;
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;
        private readonly IRoomService _roomService;
        private readonly IIdentityService _identityService;
        private readonly IWebSocketService _webSocketService;
        // private readonly IObjectStorageService _objectStorageService;
        
        public RecordController(IRecordService recordService, IMapper mapper, IUriService uriService, IIdentityService identityService, IRoomService roomService, IWebSocketService webSocketService)
        {
            _recordService = recordService;
            _mapper = mapper;
            _uriService = uriService;
            _identityService = identityService;
            _roomService = roomService;
            _webSocketService = webSocketService;
        }

        [HttpGet(ApiRoutes.Records.GetAll)]
        [Cached(600)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationQuery<OrderByEnums.RecordOrderBy> paginationQuery)
        {
            var pagination = _mapper.Map<PaginationFilter<OrderByEnums.RecordOrderBy>>(paginationQuery);
            var records = await _recordService.GetRecordsAsync(pagination);
            var recordResponses = _mapper.Map<List<RecordResponse>>(records);

            if (pagination is null || pagination.PageNumber < 1 || pagination.PageSize < 1)
            {
                return Ok(new PagedResponse<RecordResponse, OrderByEnums.RecordOrderBy>(recordResponses));
            }
            
            var pagedResponse = PaginationHelpers.CreatePagedResponse(_uriService, pagination, recordResponses);

            return Ok(pagedResponse);
        }

        [HttpGet(ApiRoutes.Records.GetFile)]
        [Cached(600)]
        public async Task<IActionResult> GetRecordFile()
        {
            // _objectStorageService.ListObjects("test", "Database.tdb").Subscribe(item => Console.WriteLine($"Object: {item.Key}"),
            //     ex => Console.WriteLine($"OnError: {ex}"),
            //     () => Console.WriteLine($"Listed all objects in bucket\n"));
            throw new NotImplementedException();
        }
 
        [HttpPost(ApiRoutes.Records.PostFile)]
        public async Task<IActionResult> PostRecordFile(IFormFile file)
        {
            Console.WriteLine(file.ContentType);
            var list = await VirtoolsArray.CreateListAsync(file.OpenReadStream());
            return Ok(list[0].SheetName);
        }
        
        [HttpGet(ApiRoutes.Records.Get)]
        [Cached(600)]
        public async Task<IActionResult> Get([FromRoute] Guid recordId)
        {
            var record = await _recordService.GetRecordByIdAsync(recordId);
            
            if (record is null)
                return NotFound();
            
            return Ok(new Response<RecordResponse>(_mapper.Map<RecordResponse>(record)));
        }

        [HttpGet(ApiRoutes.Records.GetByRoom)]
        public async Task<IActionResult> GetByRoom([FromRoute] Guid roomId, [FromQuery] PaginationQuery<OrderByEnums.RecordOrderBy> paginationQuery)
        {
            var room = await _roomService.GetRoomsByIdAsync(roomId);
            if (room is null)
                return NotFound(new {error = "Cannot find such room."});
            var pagination = _mapper.Map<PaginationFilter<OrderByEnums.RecordOrderBy>>(paginationQuery);
            var records = await _recordService.GetRecordByRoomAsync(room.Id);
            var recordResponses = _mapper.Map<List<RecordResponse>>(records);

            if (pagination is null)
            {
                return Ok(recordResponses);
            }
            
            if (pagination.PageNumber < 1 || pagination.PageSize < 1)
            {
                return Ok(new PagedResponse<RecordResponse, OrderByEnums.RecordOrderBy>(recordResponses));
            }
            
            var pagedResponse = PaginationHelpers.CreatePagedResponse(_uriService, pagination, recordResponses);

            return Ok(pagedResponse);
        }

        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        [HttpPut(ApiRoutes.Records.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid recordId, [FromBody] UpdateRecordRequest request)
        {
            var userOwnsPost = await _recordService.UserOwnsRecordAsync(recordId, HttpContext.GetUserId());
            var userIsAdmin = HttpContext.User.IsInRole("Admin");
            var userEligibleToUpdate = userOwnsPost || userIsAdmin;

            if (!userEligibleToUpdate)
            {
                return BadRequest(new {error = "You do not own this record."});
            }

            var record = await _recordService.GetRecordByIdAsync(recordId);
            
            record.Remark = request.Remark;
            record.TimeModified = DateTime.Now;

            var updated = await _recordService.UpdateRecordAsync(record);

            if (updated)
                return Ok(new Response<RecordResponse>(_mapper.Map<RecordResponse>(record)));

            return NotFound();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        [HttpDelete(ApiRoutes.Records.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid recordId)
        {
            var userOwnsPost = await _recordService.UserOwnsRecordAsync(recordId, HttpContext.GetUserId());
            var userIsAdmin = HttpContext.User.IsInRole("Admin");
            
            if (!userOwnsPost && !userIsAdmin)
            {
                return BadRequest(new {error = "You do not own this record and you are not an admin."});
            }

            var deleted = await _recordService.DeleteRecordAsync(recordId);

            if (deleted)
                return NoContent();

            return NotFound();
        }

        // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        // [HttpDelete(ApiRoutes.Records.DeleteAsAdmin)]
        // public async Task<IActionResult> DeleteAsAdmin([FromRoute] Guid recordId)
        // {
        //     return await DeletePost(recordId);
        // }

        // private async Task<IActionResult> DeletePost(Guid recordId)
        // {
        //     var deleted = await _recordService.DeleteRecordAsync(recordId);
        //
        //     if (deleted)
        //         return NoContent();
        //
        //     return NotFound();
        // }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        [HttpPost(ApiRoutes.Records.Create)]
        public async Task<IActionResult> Create([FromBody] CreateRecordRequest recordRequest)
        {
            return await CreateDefiningUser(recordRequest, Guid.Parse(HttpContext.GetUserId()));
        }
        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPost(ApiRoutes.Records.CreateDefiningUser)]
        public async Task<IActionResult> CreateDefiningUser([FromBody] CreateRecordRequest recordRequest, [FromRoute] Guid userId)
        {
            if (!await _identityService.UserExistsAsync(userId))
                return BadRequest(new {error = "User does not exist."});

            if (!string.IsNullOrEmpty(recordRequest.RoomId))
            {
                var room = await _roomService.GetRoomsByIdAsync(Guid.Parse(recordRequest.RoomId));
                if (room is null) 
                    return BadRequest(new { error = "Room does not exist." });

                if (room.Status != Status.Running)
                    return BadRequest(new { error = "Room requested is not running thus cannot submit." });
            }

            var record = _mapper.Map<Record>(recordRequest);
            record.UserId = userId.ToString();
            record.TimeCreated = DateTime.Now;
            record.TimeModified = DateTime.Now;
            
            if (record.Id == Guid.Empty)
                record.Id = Guid.NewGuid();

            await _recordService.CreateRecordAsync(record);
            
            var locationUri = _uriService.GetRecordUri(record.Id.ToString());
            var recordResponse = new Response<RecordResponse>(_mapper.Map<RecordResponse>(await _recordService.GetRecordByIdAsync(record.Id)));
            
            await _webSocketService.Broadcast(JsonConvert.SerializeObject(recordResponse),
                data => !string.IsNullOrEmpty(recordRequest.RoomId) && data.Values.ContainsKey("roomId") &&
                                recordRequest.RoomId == data.Values["roomId"]?.ToString());

            return Created(locationUri, recordResponse);
        }
    }
}
