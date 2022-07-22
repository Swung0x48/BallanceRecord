using System;
using AutoMapper;
using BallanceRecordApi.Contracts.V1.Responses;
using BallanceRecordApi.Domain;
using Microsoft.AspNetCore.Identity;

namespace BallanceRecordApi.Mapping
{
    public class DomainToResponseProfile: Profile
    {
        public DomainToResponseProfile()
        {
            CreateMap<Record, RecordResponse>()
                .ForMember(dest => dest.Username,
                    opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.Duration,
                    opt => opt.MapFrom(src => src.Duration.TotalMilliseconds));
            CreateMap<Room, RoomResponse>()
                .ForPath(dest => dest.Host.Username,
                    opt => opt.MapFrom(src => src.User.UserName))
                .ForPath(dest => dest.Host.UserId,
                    opt => opt.MapFrom(src => src.Id));
            CreateMap<IdentityUser, UserInfoResponse>();
        }
    }
}