using System.Collections.Generic;
using BallanceRecordApi.Domain;

namespace BallanceRecordApi.Contracts.V1.Responses
{
    public class PagedResponse<TData, TOrderBy>
    {
        public PagedResponse() {}

        public PagedResponse(IEnumerable<TData> data)
        {
            Data = data;
        }

        public IEnumerable<TData> Data { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public TOrderBy? OrderBy { get; set; }
        public string NextPage { get; set; }
        public string PreviousPage { get; set; }
    }
}