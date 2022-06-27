using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BallanceRecordApi.Domain;

namespace BallanceRecordApi.Services
{
    public interface IRecordService
    {
        Task<List<Record>> GetRecordsAsync(PaginationFilter<RecordOrderBy> paginationFilter = null);
        Task<Record> GetRecordByIdAsync(Guid recordId);
        Task<bool> UpdateRecordAsync(Record recordToUpdate);
        Task<bool> DeleteRecordAsync(Guid recordId);
        Task<bool> CreateRecordAsync(Record record);
        Task<bool> UserOwnsRecordAsync(Guid recordId, string userId);
    }
}