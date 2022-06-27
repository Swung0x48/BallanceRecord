namespace BallanceRecordApi.Domain
{
    public enum RecordOrderBy
    {
        HighScore,
        SpeedRun
    }
    public class PaginationFilter<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public T OrderBy { get; set; }
    }
}