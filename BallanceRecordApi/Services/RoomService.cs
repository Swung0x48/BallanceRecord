using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BallanceRecordApi.Data;
using BallanceRecordApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace BallanceRecordApi.Services;

public class RoomService: IRoomService
{
    private readonly DataContext _dataContext;

    public RoomService(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<List<Room>> GetRoomsAsync(PaginationFilter<OrderByEnums.RoomOrderBy> paginationFilter = null)
    {
        var rooms = _dataContext.Rooms;

        if (paginationFilter is null)
        {
            return await rooms.ToListAsync();
        }

        var skipSize = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
        return await rooms.Skip(skipSize).Take(paginationFilter.PageSize).ToListAsync();
    }

    public async Task<Room> GetRoomsByIdAsync(Guid roomId)
    {
        return await _dataContext.Rooms.SingleOrDefaultAsync(x => x.Id == roomId);
    }

    public async Task<bool> CreateRoomAsync(Room room)
    {
        await _dataContext.Rooms.AddAsync(room);
        var entriesAffected = await _dataContext.SaveChangesAsync();
        return entriesAffected > 0;
    }
    
    public async Task<bool> UpdateRoomAsync(Room roomToUpdate)
    {
        _dataContext.Rooms.Update(roomToUpdate);
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

    public async Task<bool> PutRoomIntoStatus(Guid roomId, Status status)
    {
        var room = await _dataContext.Rooms.SingleOrDefaultAsync(x => x.Id == roomId);
        if (room is null || room.Status == status)
        {
            return false;
        }

        room.Status = status;
        
        var updated = await UpdateRoomAsync(room);
        return updated;
    }

    public async Task<bool> StartRoomAsync(Guid roomId)
    {
        return await PutRoomIntoStatus(roomId, Status.Running);
    }

    public async Task<bool> EndRoomAsync(Guid roomId)
    {
        return await PutRoomIntoStatus(roomId, Status.Ended);
    }

    public async Task<string> GetWhichUserHostsRoom(Guid roomId)
    {
        var room = await _dataContext.Rooms.AsNoTracking().SingleOrDefaultAsync(x => x.Id == roomId);

        return room?.RoomHostUserId;
    }
}