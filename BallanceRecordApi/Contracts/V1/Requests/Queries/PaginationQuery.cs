using BallanceRecordApi.Domain;

namespace BallanceRecordApi.Contracts.V1.Requests.Queries
{
    public class PaginationQuery<T>
    {
        public PaginationQuery()
        {
            PageNumber = 1;
            PageSize = 100; // TODO: Maybe put this in config?
        }

        public PaginationQuery(int pageNumber, int pageSize, T orderBy)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            OrderBy = orderBy;
        }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public T OrderBy { get; set; }
    }
}