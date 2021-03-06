using System.Collections.Generic;
using System.Linq;
using BallanceRecordApi.Contracts.V1.Requests.Queries;
using BallanceRecordApi.Contracts.V1.Responses;
using BallanceRecordApi.Domain;
using BallanceRecordApi.Services;

namespace BallanceRecordApi.Helpers
{
    public class PaginationHelpers
    {
        public static PagedResponse<T> CreatePagedResponse<T>(IUriService uriService, PaginationFilter pagination, List<T> responses)
        {
            var nextPage = pagination.PageNumber >= 1
                ? uriService.GetAllRecordsUri(new PaginationQuery(pagination.PageNumber + 1, pagination.PageSize, pagination.OrderBy)).ToString()
                : null;
            
            var previousPage = pagination.PageNumber - 1 >= 1
                ? uriService.GetAllRecordsUri(new PaginationQuery(pagination.PageNumber - 1, pagination.PageSize, pagination.OrderBy)).ToString()
                : null;
            
            return new PagedResponse<T>
            {
                Data = responses,
                PageNumber = pagination.PageNumber >= 1 ? pagination.PageNumber : (int?)null,
                PageSize = pagination.PageSize >= 1 ? pagination.PageSize : (int?)null,
                OrderBy = pagination.OrderBy, 
                NextPage = responses.Any() ? nextPage : null,
                PreviousPage = previousPage
            };
        }
    }
}