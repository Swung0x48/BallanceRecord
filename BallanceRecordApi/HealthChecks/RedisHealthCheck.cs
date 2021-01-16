using System;
using System.Threading;
using System.Threading.Tasks;
using BallanceRecordApi.Cache;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace BallanceRecordApi.HealthChecks
{
    public class RedisHealthCheck: IHealthCheck
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly RedisCacheOptions _redisCacheOptions;

        public RedisHealthCheck(RedisCacheOptions redisCacheOptions, IConnectionMultiplexer connectionMultiplexer = null)
        {
            _redisCacheOptions = redisCacheOptions;
            if (!redisCacheOptions.Enabled)
            {
                _connectionMultiplexer = null;
                return;
            }

            _connectionMultiplexer = connectionMultiplexer;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            if (!_redisCacheOptions.Enabled)
                return await Task.FromResult(HealthCheckResult.Degraded("Redis cache is explicitly disabled."));
            
            try
            {
                var database = _connectionMultiplexer.GetDatabase();
                database.StringGet("something");
                return await Task.FromResult(HealthCheckResult.Healthy());
            }
            catch (Exception exception)
            {
                return await Task.FromResult(HealthCheckResult.Unhealthy(exception.Message));
            }
        }
    }
}