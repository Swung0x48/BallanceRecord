using System;
using System.Collections.Generic;
using System.Linq;
using BallanceRecordApi.Domain;

namespace BallanceRecordApi.Services
{
    public class RecordService: IRecordService
    {
        private List<Record> _records;
        
        public RecordService()
        {
            _records = new List<Record>();
            for (var i = 0; i < 5; i++) // TODO: Remove these before deployment.
            {
                _records.Add(new Record
                {
                    Id = Guid.NewGuid(),
                    Name = $"Record Name {i}"
                });
            }
        }

        public List<Record> GetRecords()
        {
            return _records;
        }

        public Record GetRecordById(Guid recordId)
        {
            return _records.SingleOrDefault(x => x.Id == recordId);
        }
        public bool UpdateRecord(Record recordToUpdate)
        {
            var exists = !(GetRecordById(recordToUpdate.Id) is null);
            if (!exists)
                return false;

            var index = _records.FindIndex(x => x.Id == recordToUpdate.Id);
            _records[index] = recordToUpdate;
            return true;
        }
    }
}