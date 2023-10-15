using Microsoft.AspNetCore.Mvc;
using System.Globalization;

[Route("api/predictions")]
[ApiController]
public class PredictionController : ControllerBase
{
    private List<UserData> UserDataList;
    private List<int> totalOnlineUsersList;

    public PredictionController()
    {
        string filePath = "C:\\Users\\user\\source\\repos\\IntegrationTests\\IntegrationTests\\Data.txt";
        UserDataList = ParseTextFile(filePath);

        // Extract "Total online users" values
        totalOnlineUsersList = UserDataList.Select(userData => userData.TotalOnlineUsers).ToList();
    }
    private readonly Random random = new Random();

    [HttpGet("users")]
    public IActionResult PredictUsers([FromQuery] string date)
    {
        int minTotalOnlineUsers = totalOnlineUsersList.Min();
        int maxTotalOnlineUsers = totalOnlineUsersList.Max();

        if (DateTime.TryParseExact(date, "yyyy-dd-MM-HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime targetDate))
        {
            DateTime targetDateOnly = targetDate.Date;

            var relevantData = UserDataList
                .Where(userData => userData.Time.Date == targetDateOnly)
                .ToList();

            if (relevantData.Count > 0)
            {
                return Ok(new { onlineUsers = random.Next(minTotalOnlineUsers, maxTotalOnlineUsers + 1) });
            }
        }
        int randomOnlineUsers = random.Next(minTotalOnlineUsers, maxTotalOnlineUsers + 1);

        return Ok(new { onlineUsers = randomOnlineUsers });
    }


    private List<UserData> ParseTextFile(string filePath)
    {
        List<UserData> userDataList = new List<UserData>();

        string[] lines = System.IO.File.ReadAllLines(filePath);

        foreach (string line in lines)
        {
            if (line.Contains("Total online users:"))
            {
                string[] parts = line.Split(';');
                if (parts.Length == 2)
                {
                    UserData userData = new UserData();
                    userData.Time = DateTime.ParseExact(parts[0].Trim(), "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    string totalOnlineUsersString = parts[1].Trim().Replace("Total online users:", "").Trim();
                    if (int.TryParse(totalOnlineUsersString, out int totalOnlineUsers))
                    {
                        userData.TotalOnlineUsers = totalOnlineUsers;
                    }
                    userDataList.Add(userData);
                }
            }
        }

        return userDataList;
    }

    public class UserData
    {
        public DateTime Time { get; set; }
        public int TotalOnlineUsers { get; set; }
    }
}
