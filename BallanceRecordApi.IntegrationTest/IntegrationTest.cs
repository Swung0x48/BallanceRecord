using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BallanceRecordApi.Contracts.V1;
using BallanceRecordApi.Contracts.V1.Requests;
using BallanceRecordApi.Contracts.V1.Responses;
using BallanceRecordApi.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BallanceRecordApi.IntegrationTest
{
    public class IntegrationTest
    {
        protected readonly HttpClient TestClient;
        
        protected IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var descriptor = services.SingleOrDefault(x => 
                            x.ServiceType == typeof(DbContextOptions<DataContext>)
                        );
                        if (descriptor != null) { services.Remove(descriptor); }
                        // services.RemoveAll(typeof(DataContext));
                        services.AddDbContext<DataContext>(options =>
                        {
                            options.UseInMemoryDatabase("TestDb");
                        });
                    });
                });
            TestClient = appFactory.CreateClient();
        }

        protected async Task AuthenticateAsync()
        {
            TestClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("bearer", await GetJwtAsync());
        }

        private async Task<string> GetJwtAsync()
        {
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Register, new UserRegistrationRequest
            {
                Email = "test@example.com",
                Username = "test",
                Password = "Temp233"
            });

            var succeed = response.IsSuccessStatusCode;
            
            var registrationResponse = await response.Content.ReadAsAsync<AuthSuccessResponse>();

            return registrationResponse.Token;
        }

        protected async Task<RecordResponse> CreateRecordAsync(CreateRecordRequest request)
        {
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Records.Create, request);
            return await response.Content.ReadAsAsync<RecordResponse>();
        }
    }
}