using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BallanceRecordApi.Data;
using BallanceRecordApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace BallanceRecordApi.Services;

public class LevelService: ILevelService
{
    private readonly DataContext _dataContext;
    public LevelService(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    public async Task<List<Level>> GetLevelsAsync(PaginationFilter<OrderByEnums.LevelOrderBy> paginationFilter = null)
    {
        var levels = _dataContext.Levels;
        if (paginationFilter is null)
        {
            return await levels.ToListAsync();
        }
        var skipSize = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
        return await levels
            .OrderBy(x => x.Difficulty)
            .Skip(skipSize)
            .Take(paginationFilter.PageSize)
            .ToListAsync();
    }

    public async Task<Level> GetLevelByIdAsync(Guid levelId)
    {
        return await _dataContext.Levels.FirstOrDefaultAsync(x => x.Id == levelId);
    }

    public async Task<bool> UpdateLevelAsync(Level levelToUpdate)
    {
        _dataContext.Levels.Update(levelToUpdate);
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

    public async Task<bool> CreateLevelAsync(Level room)
    {
        await _dataContext.Levels.AddAsync(room);
        var entriesAffected = await _dataContext.SaveChangesAsync();
        return entriesAffected > 0;
    }
}