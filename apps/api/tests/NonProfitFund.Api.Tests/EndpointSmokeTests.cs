using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace NonProfitFund.Api.Tests;

public sealed class EndpointSmokeTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Health_ReturnsOk()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/health");
        response.EnsureSuccessStatusCode();
    }
}

