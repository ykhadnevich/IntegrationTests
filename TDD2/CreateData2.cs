namespace Request1;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;



public interface IHttpClientWrapper
{
    Task<HttpResponseMessage> GetAsync(string requestUri);
}

public class HttpClientWrapper : IHttpClientWrapper
{
    private readonly HttpClient _httpClient;

    public HttpClientWrapper()
    {
        _httpClient = new HttpClient();
    }

    public async Task<HttpResponseMessage> GetAsync(string requestUri)
    {
        return await _httpClient.GetAsync(requestUri);
    }
}

public class Request
{
    private readonly IHttpClientWrapper _httpClientWrapper;

    public Request(IHttpClientWrapper httpClientWrapper)
    {
        _httpClientWrapper = httpClientWrapper;
    }

    public static async Task Main(string[] args)
    {
        var program = new Request(new HttpClientWrapper());
        await program.ProcessUserInformationAsync();

    }

    public async Task ProcessUserInformationAsync()
    {
        string base_url = "https://sef.podkolzin.consulting/api/users/lastSeen";
        int offset = 0;
        int pageSize = 20;

        while (true)
        {
            UserDataResponse userDataResponse = await FetchUserDataAsync(base_url, offset);
            if (userDataResponse == null)
            {
                Console.WriteLine("Failed to retrieve data.");
                break;
            }

            foreach (UserData user in userDataResponse.data)
            {
                DisplayUserInfo(user);
            }

            if (userDataResponse.data.Count < pageSize)
            {
                break;
            }
            offset += pageSize;
        }
    }

    public async Task<UserDataResponse?> FetchUserDataAsync(string baseUrl, int offset)
    {
        try
        {
            string apiUrl = $"{baseUrl}?offset={offset}";
            HttpResponseMessage response = await _httpClientWrapper.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                string json_data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserDataResponse>(json_data);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        return null;
    }

    public string GenerateCustomGuid()
    {
        Guid newGuid = Guid.NewGuid();
        return newGuid.ToString("D").ToUpper();
    }

    public void DisplayUserInfo(UserData user)
    {
        string customGuid = GenerateCustomGuid();
        string lastSeenStatus = GetLastSeenStatus(user);
        Console.WriteLine($"ID: {user.userId}; Time: {DateTime.UtcNow}; nearestOnlineTime: {user.lastSeenDate}; wasUserOnline: {lastSeenStatus} ");

        string outputPath = "C:\\Users\\user\\source\\repos\\TDD\\TDD2\\Data2.txt";
        string outputText = $"ID:{user.userId};Time:{DateTime.UtcNow};nearestOnlineTime:{user.lastSeenDate};wasUserOnline:{lastSeenStatus}";

        File.AppendAllText(outputPath, outputText + Environment.NewLine);
        Console.WriteLine(outputText);
    }


    public static string GetLastSeenStatus(UserData user)
    {
        DateTime current_time = DateTime.UtcNow;
        bool isOnline = user.isOnline;



        if (isOnline)
        {
            return "online";
        }
        else
        {
            return "false";
        }
    }
}

public class UserDataResponse
{
    public List<UserData> data { get; set; }
}

public class UserData
{
    public string userId { get; set; }
    public string firstName { get; set; }
    public string nickname { get; set; }
    public bool isOnline { get; set; }
    public string lastSeenDate { get; set; }
}
