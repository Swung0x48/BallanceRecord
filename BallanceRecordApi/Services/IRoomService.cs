using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BallanceRecordApi.Domain;

namespace BallanceRecordApi.Services;

public interface IRoomService
{
    Task<List<Room>> GetRoomsAsync(PaginationFilter<OrderByEnums.RoomOrderBy> paginationFilter = null);
    Task<Room> GetRoomsByIdAsync(Guid roomId);
    Task<bool> UpdateRoomAsync(Room roomToUpdate);
    Task<bool> CreateRoomAsync(Room room);
    Task<bool> PutRoomIntoStatus(Guid roomId, Status status);
    Task<bool> StartRoomAsync(Guid roomId);
    Task<bool> EndRoomAsync(Guid roomId);
    Task<string> GetWhichUserHostsRoom(Guid roomId);
}