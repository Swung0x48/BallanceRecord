using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BallanceRecordApi.Contracts.V1;
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
            (await response.Content.ReadAsAsync<List<Record>>()).Should().BeEmpty();
        }
    }
}