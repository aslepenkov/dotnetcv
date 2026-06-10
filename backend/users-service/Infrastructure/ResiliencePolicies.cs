using System;
using System.Net.Http;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;

namespace UsersService.Infrastructure;

public static class ResiliencePolicies
{
    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
    }
}