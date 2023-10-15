using ikvm.runtime;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Assert = Xunit.Assert;
using TheoryAttribute = NUnit.Framework.TheoryAttribute;


public class APIIntegrationTest : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly WebApplicationFactory<Startup> _factory;

    public APIIntegrationTest(WebApplicationFactory<Startup> factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("2023-10-13-12:00:00", 0.5, "user-id-1")]
    [InlineData("2023-10-13-15:30:00", 0.7, "user-id-2")]
    public async Task PredictUserOnline_ReturnsExpectedResult(string date, double tolerance, string userId)
    {
        var client = _factory.CreateClient();
        var requestUrl = $"/api/predictions/user?date={date}&tolerance={tolerance}&userId={userId}";

        var response = await client.GetAsync(requestUrl);

        response.EnsureSuccessStatusCode(); // Status Code 200-299
        var content = await response.Content.ReadAsStringAsync();
      
        Assert.Contains("willBeOnline", content);
        Assert.Contains("onlineChance", content);
    }
}
