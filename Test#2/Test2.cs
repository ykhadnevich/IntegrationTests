using ikvm.runtime;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Assert = Xunit.Assert;
using TheoryAttribute = NUnit.Framework.TheoryAttribute;

public class UserControllerIntegrationTest : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly WebApplicationFactory<Startup> _factory;

    public UserControllerIntegrationTest(WebApplicationFactory<Startup> factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("your-user-id-1", "2023-10-13-12:00:00")]
    [InlineData("your-user-id-2", "2023-10-13-15:30:00")]
    public async Task GetUser_ReturnsExpectedResult(string userId, string date)
    {
        var client = _factory.CreateClient();
        var requestUrl = $"/api/stats/users?userId={userId}&date={date}";

        var response = await client.GetAsync(requestUrl);

        response.EnsureSuccessStatusCode(); 
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("wasUserOnline", content);
        Assert.Contains("nearestOnlineTime", content);
    }
}
