using System;
using System.Collections.Generic;
using BallanceRecordApi.Contracts.V1;
using BallanceRecordApi.Contracts.V1.Requests;
using BallanceRecordApi.Contracts.V1.Responses;
using BallanceRecordApi.Domain;
using Microsoft.AspNetCore.Mvc;

namespace BallanceRecordApi.Controllers.V1
{
    public class RecordsController: Controller
    {
        private List<Record> _records;

        public RecordsController()
        {
            _records = new List<Record>();
            for (var i = 0; i < 5; i++)
            {
                _records.Add(new Record
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = $"Record Name {i}"
                });
            }
        }

        [HttpGet(ApiRoutes.Records.GetAll)]
        public IActionResult GetAll()
        {
            return Ok(_records);
        }
        
        [HttpGet(ApiRoutes.Records.Get)]
        public IActionResult Get()
        {
            return Ok(_records);
        }

        [HttpPost(ApiRoutes.Records.Create)]
        public IActionResult Create([FromBody] CreateRecordRequest recordRequest)
        {
            var record = new Record { Id = recordRequest.Id };
            
            if (string.IsNullOrEmpty(record.Id))
                record.Id = Guid.NewGuid().ToString();
            
            _records.Add(record);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = $"{baseUrl}/{ApiRoutes.Records.Get.Replace("{recordId}", record.Id )}";
            
            var response = new RecordResponse { Id = record.Id };
            return Created(locationUri, response);
        }
    }
}
