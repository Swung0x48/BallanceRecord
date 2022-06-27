using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BallanceRecordApi.Domain;

namespace BallanceRecordApi.Services;

public interface ISessionService
{
    Task<List<Session>> GetSessionsAsync(PaginationFilter<OrderByEnums.SessionOrderBy> paginationFilter = null);
    Task<Session> GetSessionByIdAsync(Guid sessionId);
    Task<Session> GetJoinedSessionAsync();
    Task<Session> GetSessionByUserAsync();
    Task<Guid> CreateSessionAsync();
    Task<bool> StartSessionAsync();
    Task<bool> EndSessionAsync();
    Task<bool> JoinSessionAsync(Guid sessionId);
    Task<bool> LeaveSessionAsync();
}