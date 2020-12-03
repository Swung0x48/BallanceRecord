using System;
using BallanceRecordApi.Contracts.V1.Requests.Queries;

namespace BallanceRecordApi.Services
{
    public interface IUriService
    {
        Uri GetRecordUri(string recordId);

        Uri GetAllRecordsUri(PaginationQuery pagination = null);
    }
}