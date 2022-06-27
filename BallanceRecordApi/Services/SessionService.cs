using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BallanceRecordApi.Data;
using BallanceRecordApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace BallanceRecordApi.Services;

public class SessionService: ISessionService
{
    private readonly DataContext _dataContext;

    public SessionService(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<List<Session>> GetSessionsAsync(PaginationFilter<OrderByEnums.SessionOrderBy> paginationFilter = null)
    {
        var sessions = _dataContext.Sessions.Include(x => x.User);

        if (paginationFilter is null)
        {
            return await sessions.ToListAsync();
        }

        var skipSize = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
        return null;
    }

    public async Task<Session> GetSessionByIdAsync(Guid sessionId)
    {
        throw new NotImplementedException();
    }

    public async Task<Session> GetJoinedSessionAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Session> GetSessionByUserAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Guid> CreateSessionAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> StartSessionAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> EndSessionAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> JoinSessionAsync(Guid sessionId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> LeaveSessionAsync()
    {
        throw new NotImplementedException();
    }
}