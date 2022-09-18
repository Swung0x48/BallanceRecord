using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BallanceRecordApi.Domain;

namespace BallanceRecordApi.Services;

public interface ILevelService
{
    Task<List<Level>> GetLevelsAsync(PaginationFilter<OrderByEnums.LevelOrderBy> paginationFilter = null);
    Task<Level> GetLevelByIdAsync(Guid levelId);
    Task<bool> UpdateLevelAsync(Level levelToUpdate);
    Task<bool> CreateLevelAsync(Level room);
}