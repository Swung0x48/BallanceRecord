using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace BallanceRecordApi.HealthChecks
{
    public class RedisHealthCheck: IHealthCheck
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public RedisHealthCheck(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                var database = _connectionMultiplexer.GetDatabase();
                database.StringGet("something");
                return await Task.FromResult(HealthCheckResult.Healthy());
            }
            catch (Exception exception)
            {
                return await Task.FromResult(HealthCheckResult.Unhealthy(exception.Message));
                throw;
            }
        }
    }
}