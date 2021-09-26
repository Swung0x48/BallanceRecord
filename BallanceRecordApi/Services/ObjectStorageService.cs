using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Minio;
using Minio.DataModel;

namespace BallanceRecordApi.Services
{
    public class ObjectStorageService: IObjectStorageService
    {
        private readonly MinioClient _minioClient;

        public ObjectStorageService(MinioClient minioClient)
        {
            _minioClient = minioClient;
        }

        public async Task<ListAllMyBucketsResult> ListBuckets()
        {
            return await _minioClient.ListBucketsAsync();
        }

        public IObservable<Item> ListObjects(string bucketName, string prefix = null, bool recursive = true)
        {
            return _minioClient.ListObjectsAsync(bucketName, prefix, recursive);
        }
    }
}