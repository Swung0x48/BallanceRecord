namespace BallanceRecordApi.Domain
{
    public class PaginationFilter<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public T OrderBy { get; set; }
    }
}