namespace BallanceRecordApi.Domain
{
    public class PaginationFilter
    {
        public enum OrderByType
        {
            Highscore,
            Speedrun
        }
        
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public OrderByType OrderBy { get; set; }
    }
}