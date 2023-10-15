using ikvm.runtime;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Assert = Xunit.Assert;
using TheoryAttribute = NUnit.Framework.TheoryAttribute;

public class PredictionControllerIntegrationTest : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly WebApplicationFactory<Startup> _factory;

    public PredictionControllerIntegrationTest(WebApplicationFactory<Startup> factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("2023-10-13-12:00:00")]
    [InlineData("2023-10-13-15:30:00")]
    public async Task PredictUsers_ReturnsExpectedResult(string date)
    {
        var client = _factory.CreateClient();
        var requestUrl = $"/api/predictions/users?date={date}";

        var response = await client.GetAsync(requestUrl);

        response.EnsureSuccessStatusCode(); // Status Code 200-299
        var content = await response.Content.ReadAsStringAsync();
        
        Assert.Contains("onlineUsers", content);
    }
}
