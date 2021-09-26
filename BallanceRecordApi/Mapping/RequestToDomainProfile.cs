using System;
using AutoMapper;
using BallanceRecordApi.Contracts.V1.Requests;
using BallanceRecordApi.Contracts.V1.Requests.Queries;
using BallanceRecordApi.Domain;

namespace BallanceRecordApi.Mapping
{
    public class RequestToDomainProfile: Profile
    {
        public RequestToDomainProfile()
        {
            CreateMap<CreateRecordRequest, Record>()
                .ForMember(dest => dest.Duration,
                    option => 
                        option.MapFrom(src => TimeSpan.FromSeconds(src.Duration)));
            CreateMap<PaginationQuery, PaginationFilter>();
        }
    }
}