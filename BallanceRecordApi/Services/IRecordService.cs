using System;
using System.Collections.Generic;
using BallanceRecordApi.Domain;

namespace BallanceRecordApi.Services
{
    public interface IRecordService
    {
        List<Record> GetRecords();
        Record GetRecordById(Guid recordId);
    }
}