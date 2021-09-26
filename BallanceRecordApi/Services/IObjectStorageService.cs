using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Minio.DataModel;

namespace BallanceRecordApi.Services
{
    public interface IObjectStorageService
    {
        Task<ListAllMyBucketsResult> ListBuckets();

        IObservable<Item> ListObjects(string bucketName,
            string prefix = null,
            bool recursive = true);
    }
}