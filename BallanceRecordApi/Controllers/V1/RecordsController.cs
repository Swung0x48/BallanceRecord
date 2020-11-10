using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BallanceRecordApi.Contracts.V1;
using BallanceRecordApi.Contracts.V1.Requests;
using BallanceRecordApi.Contracts.V1.Responses;
using BallanceRecordApi.Domain;
using BallanceRecordApi.Extensions;
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
        
        public RecordsController(IRecordService recordService, IMapper mapper)
        {
            _recordService = recordService;
            _mapper = mapper;
        }

        [HttpGet(ApiRoutes.Records.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            var records = await _recordService.GetRecordsAsync();
            var recordResponses = _mapper.Map<List<RecordResponse>>(records);
            // var recordResponses = records.Select(record => new RecordResponse
            // {
            //     Id = record.Id,
            //     Name = record.Name,
            //     UserId = record.UserId
            // }).ToList();
            return Ok(recordResponses);
        }
        
        [HttpGet(ApiRoutes.Records.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid recordId)
        {
            var record = await _recordService.GetRecordByIdAsync(recordId);
            
            if (record is null)
                return NotFound();
            
            return Ok(_mapper.Map<RecordResponse>(record));
        }
        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPut(ApiRoutes.Records.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid recordId, [FromBody] UpdateRecordRequest request)
        {
            var userOwnsPost = await _recordService.UserOwnsPostAsync(recordId, HttpContext.GetUserId());

            if (!userOwnsPost)
            {
                return BadRequest(new {error = "You do not own this record."});
            }

            var record = await _recordService.GetRecordByIdAsync(recordId);
            record.Name = request.Name;

            var updated = await _recordService.UpdateRecordAsync(record);

            if (updated)
                return Ok(_mapper.Map<RecordResponse>(record));

            return NotFound();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete(ApiRoutes.Records.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid recordId)
        {
            var userOwnsPost = await _recordService.UserOwnsPostAsync(recordId, HttpContext.GetUserId());
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
                Name = recordRequest.Name,
                UserId = HttpContext.GetUserId(),
                Score = recordRequest.Score,
                Time = recordRequest.Time
            };
            
            if (record.Id == Guid.Empty)
                record.Id = Guid.NewGuid();

            await _recordService.CreateRecordAsync(record);
            
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = $"{baseUrl}/{ApiRoutes.Records.Get.Replace("{recordId}", record.Id.ToString() )}";
            
            return Created(locationUri, _mapper.Map<RecordResponse>(record));
        }
    }
}
