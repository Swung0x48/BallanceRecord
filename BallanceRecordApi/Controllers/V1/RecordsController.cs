using System;
using System.Threading.Tasks;
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
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _recordService.GetRecordsAsync());
        }
        
        [HttpGet(ApiRoutes.Records.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid recordId)
        {
            var record = await _recordService.GetRecordByIdAsync(recordId);
            
            if (record is null)
                return NotFound();
            
            return Ok(record);
        }
        
        [HttpPut(ApiRoutes.Records.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid recordId, [FromBody] UpdateRecordRequest request)
        {
            var record = new Record
            {
                Id = recordId,
                Name = request.Name
            };

            var updated = await _recordService.UpdateRecordAsync(record);
            
            if (updated)
                return Ok(record);

            return NotFound();
        }

        [HttpDelete(ApiRoutes.Records.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid recordId)
        {
            var deleted = await _recordService.DeleteRecordAsync(recordId);

            if (deleted)
                return NoContent();

            return NotFound();
        }

        [HttpPost(ApiRoutes.Records.Create)]
        public async Task<IActionResult> Create([FromBody] CreateRecordRequest recordRequest)
        {
            var record = new Record
            {
                Name = recordRequest.Name
            };
            
            if (record.Id == Guid.Empty)
                record.Id = Guid.NewGuid();

            await _recordService.CreateRecordAsync(record);
            
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
