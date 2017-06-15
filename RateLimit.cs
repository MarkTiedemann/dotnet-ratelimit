using System;
using System.Threading;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Primitives;

public class RateLimit
{
    public static void Main()
    {
        // use proper key-value caches in production
        var cache = new ConcurrentDictionary<int, int>();

        var limit = 5;
        var interval = 20;

        var nextReset = DateTime.Now.AddSeconds(interval);
        var resetStart = TimeSpan.Zero;
        var resetInterval = TimeSpan.FromSeconds(interval);

        new Timer((e) =>
        {
            cache.Clear();
            nextReset = DateTime.Now.AddSeconds(interval);

        }, null, resetStart, resetInterval);
        
        new WebHostBuilder()
        .UseKestrel()
        .Configure(app => app.Run(async context =>
        {
            var req = context.Request;
            var res = context.Response;

            if (!req.Headers.TryGetValue("Authorization", out StringValues auths)) {
                res.StatusCode = 401;
                return;
            }

            var auth = auths[0];
            if (!auth.StartsWith("Bearer "))
            {
                res.StatusCode = 401;
                return;
            }

            var token = auth.Substring("Bearer ".Length);

            // use a proper client id lookup in production
            var clientId = Math.Abs(token.GetHashCode());
            res.Headers.Add("X-Client-ID", clientId.ToString());

            var reset = Math.Round(nextReset.Subtract(DateTime.Now).TotalSeconds);
            res.Headers.Add("X-RateLimit-Reset", reset + "s");

            cache.TryGetValue(clientId, out int count);

            var remaining = limit - count - 1;
            // remaining will be -1 once limit is reached
            // but should still return 0 to users
            remaining = remaining == -1 ? 0 : remaining;
            res.Headers.Add("X-RateLimit-Remaining", remaining.ToString());

            if (count >= limit)
            {
                res.StatusCode = 429;
                return;
            }

            cache[clientId] = count + 1;
        }))
        .Build()
        .Run();
    }
}