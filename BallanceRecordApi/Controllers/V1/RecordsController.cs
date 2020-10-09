using System;
using System.Collections.Generic;
using System.Linq;
using BallanceRecordApi.Contracts.V1;
using BallanceRecordApi.Contracts.V1.Requests;
using BallanceRecordApi.Contracts.V1.Responses;
using BallanceRecordApi.Domain;
using BallanceRecordApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BallanceRecordApi.Controllers.V1
{
    public class RecordsController: Controller
    {

        private readonly IRecordService _recordService;

        public RecordsController(IRecordService recordService)
        {
            _recordService = recordService;
        }

        [HttpGet(ApiRoutes.Records.GetAll)]
        public IActionResult GetAll()
        {
            return Ok(_recordService.GetRecords());
        }
        
        [HttpGet(ApiRoutes.Records.Get)]
        public IActionResult Get([FromRoute] Guid recordId)
        {
            var record = _recordService.GetRecordById(recordId);
            
            if (record is null)
                return NotFound();
            
            return Ok(record);
        }
        
        [HttpPut(ApiRoutes.Records.Update)]
        public IActionResult Update([FromRoute] Guid recordId, [FromBody] UpdateRecordRequest request)
        {
            var record = _recordService.GetRecordById(recordId);
            
            if (record is null)
                return NotFound();
            
            return Ok(record);
        }

        [HttpPost(ApiRoutes.Records.Create)]
        public IActionResult Create([FromBody] CreateRecordRequest recordRequest)
        {
            var record = new Record
            {
                Id = Guid.NewGuid(),
                Name = recordRequest.Name
            };
            
            if (record.Id == Guid.Empty)
                record.Id = Guid.NewGuid();
            
            _recordService.GetRecords().Add(record);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = $"{baseUrl}/{ApiRoutes.Records.Get.Replace("{recordId}", record.Id.ToString() )}";
            
            var response = new RecordResponse
            {
                Id = record.Id,
                Name = record.Name
            };
            return Created(locationUri, response);
        }
    }
}
