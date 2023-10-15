global using NUnit.Framework;
using ikvm.runtime;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Assert = Xunit.Assert;
using TheoryAttribute = NUnit.Framework.TheoryAttribute;


public class StatsControllerIntegrationTest : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly WebApplicationFactory<Startup> _factory;

    public StatsControllerIntegrationTest(WebApplicationFactory<Startup> factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("2023-10-13-12:00:00")]
    [InlineData("2023-10-13-15:30:00")]
    public async Task GetUsers_ReturnsExpectedResult(string date)
    {
        
        var client = _factory.CreateClient();
        var requestUrl = $"/api/stats/users?date={date}";

        
        var response = await client.GetAsync(requestUrl);

        
        response.EnsureSuccessStatusCode(); 
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("usersOnline:", content);
    }
}