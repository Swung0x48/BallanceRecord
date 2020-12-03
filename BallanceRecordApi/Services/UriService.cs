using System;
using BallanceRecordApi.Contracts.V1;
using BallanceRecordApi.Contracts.V1.Requests.Queries;
using Microsoft.AspNetCore.WebUtilities;

namespace BallanceRecordApi.Services
{
    public class UriService: IUriService
    {
        private readonly string _baseUri;
        
        public UriService(string baseUri)
        {
            _baseUri = baseUri;
        }
        
        public Uri GetRecordUri(string recordId)
        {
            return new Uri(_baseUri + ApiRoutes.Records.Get.Replace("{recordId}", recordId));
        }

        public Uri GetAllRecordsUri(PaginationQuery pagination = null)
        {
            var uri = new Uri(_baseUri);

            if (pagination is null)
            {
                return uri;
            }

            var modifiedUri = QueryHelpers.AddQueryString(_baseUri, "pageNumeber", pagination.PageNumber.ToString());
            modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", pagination.PageSize.ToString());

            return new Uri(modifiedUri);
        }
    }
}