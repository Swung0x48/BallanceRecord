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
                .ForMember(dest => dest.RoomId,
                    option =>
                        option.PreCondition(src => !string.IsNullOrEmpty(src.RoomId)))
                .ForMember(dest => dest.Duration,
                    option =>
                        option.MapFrom(src => TimeSpan.FromMilliseconds(src.Duration)));
            CreateMap<CreateRoomRequest, Room>();
            CreateMap<PaginationQuery<OrderByEnums.RecordOrderBy>, PaginationFilter<OrderByEnums.RecordOrderBy>>();
            CreateMap<PaginationQuery<OrderByEnums.RoomOrderBy>, PaginationFilter<OrderByEnums.RoomOrderBy>>();
        }
    }
}