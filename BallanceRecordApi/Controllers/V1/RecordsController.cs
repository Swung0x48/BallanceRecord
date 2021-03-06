﻿using System;
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
using Microsoft.AspNetCore.Mvc;

namespace BallanceRecordApi.Controllers.V1
{
    public class RecordsController: Controller
    {
        private readonly IRecordService _recordService;
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;
        
        public RecordsController(IRecordService recordService, IMapper mapper, IUriService uriService)
        {
            _recordService = recordService;
            _mapper = mapper;
            _uriService = uriService;
        }

        [HttpGet(ApiRoutes.Records.GetAll)]
        [Cached(600)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationQuery paginationQuery)
        {
            var pagination = _mapper.Map<PaginationFilter>(paginationQuery);
            var records = await _recordService.GetRecordsAsync(pagination);
            var recordResponses = _mapper.Map<List<RecordResponse>>(records);

            if (pagination is null || pagination.PageNumber < 1 || pagination.PageSize < 1)
            {
                return Ok(new PagedResponse<RecordResponse>(recordResponses));
            }
            
            var pagedResponse = PaginationHelpers.CreatePagedResponse(_uriService, pagination, recordResponses);
            
            return Ok(pagedResponse);
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
        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPut(ApiRoutes.Records.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid recordId, [FromBody] UpdateRecordRequest request)
        {
            var userOwnsPost = await _recordService.UserOwnsRecordAsync(recordId, HttpContext.GetUserId());

            if (!userOwnsPost)
            {
                return BadRequest(new {error = "You do not own this record."});
            }

            var record = await _recordService.GetRecordByIdAsync(recordId);
            record.Remark = request.Remark;

            var updated = await _recordService.UpdateRecordAsync(record);

            if (updated)
                return Ok(new Response<RecordResponse>(_mapper.Map<RecordResponse>(record)));

            return NotFound();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost(ApiRoutes.Records.Create)]
        public async Task<IActionResult> Create([FromBody] CreateRecordRequest recordRequest)
        {
            var record = new Record
            {
                Remark = recordRequest.Remark,
                UserId = HttpContext.GetUserId(),
                MapHash = recordRequest.MapHash,
                Score = recordRequest.Score,
                Time = recordRequest.Time
            };
            
            if (record.Id == Guid.Empty)
                record.Id = Guid.NewGuid();

            await _recordService.CreateRecordAsync(record);
            
            var locationUri = _uriService.GetRecordUri(record.Id.ToString());
            var recordResponse = new Response<RecordResponse>(_mapper.Map<RecordResponse>(await _recordService.GetRecordByIdAsync(record.Id)));
            return Created(locationUri, recordResponse);
        }
    }
}
