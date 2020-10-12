using System;

namespace BallanceRecordApi.Options
{
    public class JwtOptions
    {
        public string Secret { get; set; }
        public TimeSpan TokenLifetime { get; set; }
    }
}