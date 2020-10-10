using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BallanceRecordApi.Data;
using BallanceRecordApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace BallanceRecordApi.Services
{
    public class RecordService: IRecordService
    {
        private List<Record> _records;
        private readonly DataContext _dataContext;
        
        public RecordService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<Record>> GetRecordsAsync()
        {
            return await _dataContext.Records.ToListAsync();
        }

        public async Task<Record> GetRecordByIdAsync(Guid recordId)
        {
            return await _dataContext.Records.SingleOrDefaultAsync(x => x.Id == recordId);
        }

        public async Task<bool> CreateRecordAsync(Record record)
        {
            await _dataContext.Records.AddAsync(record);
            var hasCreated = await _dataContext.SaveChangesAsync();
            return hasCreated > 0;
        }
        public async Task<bool> UpdateRecordAsync(Record recordToUpdate)
        {
            _dataContext.Records.Update(recordToUpdate);
            var hasUpdated = await _dataContext.SaveChangesAsync();
            return hasUpdated > 0;
        }

        public async Task<bool> DeleteRecordAsync(Guid postId)
        {
            var post = await GetRecordByIdAsync(postId);
            _dataContext.Records.Remove(post);

            var hasDeleted = await _dataContext.SaveChangesAsync();
            return hasDeleted > 0;
        }
    }
}