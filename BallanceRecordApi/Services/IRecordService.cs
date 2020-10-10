using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BallanceRecordApi.Domain;

namespace BallanceRecordApi.Services
{
    public interface IRecordService
    {
        Task<List<Record>> GetRecordsAsync();
        Task<Record> GetRecordByIdAsync(Guid recordId);
        Task<bool> UpdateRecordAsync(Record recordToUpdate);
        Task<bool> DeleteRecordAsync(Guid postId);
        Task<bool> CreateRecordAsync(Record record);
    }
}