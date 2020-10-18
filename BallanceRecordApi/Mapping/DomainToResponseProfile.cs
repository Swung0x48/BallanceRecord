using AutoMapper;
using BallanceRecordApi.Contracts.V1.Responses;
using BallanceRecordApi.Domain;

namespace BallanceRecordApi.Mapping
{
    public class DomainToResponseProfile: Profile
    {
        public DomainToResponseProfile()
        {
            CreateMap<Record, RecordResponse>();
        }
    }
}