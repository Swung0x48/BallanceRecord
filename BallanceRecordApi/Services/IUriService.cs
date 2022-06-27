using System;
using BallanceRecordApi.Contracts.V1.Requests.Queries;

namespace BallanceRecordApi.Services
{
    public interface IUriService
    {
        Uri GetRecordUri(string recordId);

        Uri GetAllRecordsUri<T>(PaginationQuery<T> pagination = null);

        Uri GetUserConfirmationUri(string userId, string token);
    }
}