using Newtonsoft.Json;
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

public class Program1
{
    public static async Task Main(string[] args)
    {
        var program = new Program1(new HttpClientWrapper());

        int totalOnlineUsers = await program.CountOnlineUsersAsync();

        string outputPath = "C:\\Users\\user\\source\\repos\\TDD\\TDD\\Data.txt";

        string outputText = $"{DateTime.UtcNow}; Total online users: {totalOnlineUsers} ";

        File.AppendAllText(outputPath, outputText + Environment.NewLine);

        Console.WriteLine(outputText);


    }

    private readonly IHttpClientWrapper _httpClientWrapper;

    public Program1(IHttpClientWrapper httpClientWrapper)
    {
        _httpClientWrapper = httpClientWrapper;
    }

    public async Task<int> CountOnlineUsersAsync()
    {
        string base_url = "https://sef.podkolzin.consulting/api/users/lastSeen";
        int offset = 0;
        int pageSize = 20;
        int totalOnlineUsers = 0;

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
                if (user.isOnline)
                {
                    totalOnlineUsers++;
                }
            }

            if (userDataResponse.data.Count < pageSize)
            {
                break;
            }
            offset += pageSize;
        }
        return totalOnlineUsers;
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
    private static readonly HttpClient httpClient = new HttpClient();
}
