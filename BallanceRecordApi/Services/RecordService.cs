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
        private readonly DataContext _dataContext;
        
        public RecordService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<Record>> GetRecordsAsync(PaginationFilter paginationFilter = null)
        {
            var records = _dataContext.Records.Include(x => x.User);
            
            if (paginationFilter is null)
            {
                return await records.ToListAsync();
            }

            var skipSize = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;

            return paginationFilter.OrderBy switch
            {
                PaginationFilter.OrderByType.HighScore => await records
                    .OrderByDescending(x => x.Score)
                    .ThenBy(xx => xx.Duration)
                    .Skip(skipSize)
                    .Take(paginationFilter.PageSize)
                    .ToListAsync(),
                PaginationFilter.OrderByType.SpeedRun => await records
                    .OrderBy(xx => xx.Duration)
                    .ThenByDescending(x => x.Score)
                    .Skip(skipSize)
                    .Take(paginationFilter.PageSize)
                    .ToListAsync(),
                _ => await records.Skip(skipSize).Take(paginationFilter.PageSize).ToListAsync()
            };
        }

        public async Task<Record> GetRecordByIdAsync(Guid recordId)
        {
            return await _dataContext.Records.Include(x => x.User).SingleOrDefaultAsync(x => x.Id == recordId);
        }

        public async Task<bool> CreateRecordAsync(Record record)
        {
            await _dataContext.Records.AddAsync(record);
            var hasCreated = await _dataContext.SaveChangesAsync();
            return hasCreated > 0;
        }

        public async Task<bool> UserOwnsRecordAsync(Guid recordId, string userId)
        {
            var record = await _dataContext.Records.AsNoTracking().SingleOrDefaultAsync(x => x.Id == recordId);

            if (record is null)
                return false;
            

            if (record.UserId != userId)
                return false;

            return true;
        }

        public async Task<bool> UpdateRecordAsync(Record recordToUpdate)
        {
            _dataContext.Records.Update(recordToUpdate);
            try
            {
                var hasUpdated = await _dataContext.SaveChangesAsync();
                return hasUpdated > 0;
            }
            catch (DbUpdateConcurrencyException e)
            {
                await e.Entries.Single().ReloadAsync();
                var hasUpdated = await _dataContext.SaveChangesAsync();
                return hasUpdated > 0;
            }
        }

        public async Task<bool> DeleteRecordAsync(Guid recordId)
        {
            var record = await GetRecordByIdAsync(recordId);

            if (record is null)
                return false;
            
            _dataContext.Records.Remove(record);
            var hasDeleted = await _dataContext.SaveChangesAsync();
            return hasDeleted > 0;
        }
    }
}