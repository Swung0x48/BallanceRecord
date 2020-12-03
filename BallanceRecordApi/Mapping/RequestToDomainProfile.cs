using AutoMapper;
using BallanceRecordApi.Contracts.V1.Requests.Queries;
using BallanceRecordApi.Domain;

namespace BallanceRecordApi.Mapping
{
    public class RequestToDomainProfile: Profile
    {
        public RequestToDomainProfile()
        {
            CreateMap<PaginationQuery, PaginationFilter>();
        }
    }
}