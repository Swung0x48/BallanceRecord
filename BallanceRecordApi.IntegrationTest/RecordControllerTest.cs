using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BallanceRecordApi.Contracts.V1;
using BallanceRecordApi.Contracts.V1.Requests;
using FluentAssertions;
using Xunit;

namespace BallanceRecordApi.IntegrationTest
{
    public class RecordControllerTest: IntegrationTest
    {
        [Fact]
        public async Task GetAll_WithoutAnyRecords_ReturnsEmptyResponse()
        {
            // Arrange
            await AuthenticateAsync();
            
            // Act
            var response = await TestClient.GetAsync(ApiRoutes.Records.GetAll);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            (await response.Content.ReadAsAsync<List<BallanceRecordApi.Domain.Record>>()).Should().BeEmpty();
        }

        [Fact]
        public async Task Get_RecordExists_ReturnsRecord()
        {
            // Arrange
            await AuthenticateAsync();
            var recordCreated = await CreateRecordAsync(new CreateRecordRequest {Name = "Testinggggg"});

            // Act
            var response = await TestClient.GetAsync(ApiRoutes.Records.Get.Replace("{recordId}", recordCreated.Id.ToString()));
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnedRecord = await response.Content.ReadAsAsync<BallanceRecordApi.Domain.Record>();
            returnedRecord.Id.Should().Be(recordCreated.Id);
            returnedRecord.Name.Should().Be("Testinggggg");
        }
    }
}