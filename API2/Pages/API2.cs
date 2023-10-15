using System.Globalization;
using Microsoft.AspNetCore.Mvc;

[Route("api/stats")]
[ApiController]
public class UserController : ControllerBase
{
    private List<UserData> UserDataList;

    public UserController()
    {
        string filePath = "C:\\Users\\user\\source\\repos\\TDD\\TDD2\\Data2.txt";
        UserDataList = ParseTextFile(filePath);
    }

    [HttpGet("users")]
    public IActionResult GetUser([FromQuery] string userId, [FromQuery] string date)
    {
        if (UserDataList.Count == 0)
        {
            return NotFound(); 
        }
        if (DateTime.TryParseExact(date, "yyyy-MM-dd-HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
        {
            var userData = UserDataList.Find(user => user.ID == userId && user.Time == dateTime);
            if (userData != null)
            {
                return Ok(new
                {
                    wasUserOnline = userData.WasUserOnline,
                    nearestOnlineTime = userData.NearestOnlineTime
                });
            }
            else
            {
                return NotFound(); 
            }
        }

        return BadRequest("Invalid date format.");
    }

    private List<UserData> ParseTextFile(string filePath)
    {
        List<UserData> userDataList = new List<UserData>();

        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split(';');
                if (parts.Length == 4)
                {
                    UserData userData = new UserData();

                    foreach (string part in parts)
                    {
                        if (part.StartsWith("ID:"))
                        {
                            userData.ID = part.Substring(3);
                        }
                        else if (part.StartsWith("Time:"))
                        {
                            userData.Time = DateTime.ParseExact(part.Substring(5), "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        }
                        else if (part.StartsWith("nearestOnlineTime:"))
                        {
                            userData.NearestOnlineTime = ParseDateTimeFromPart(part, "nearestOnlineTime:");
                        }

                        else if (part.StartsWith("wasUserOnline:"))
                        {
                            userData.WasUserOnline = part.Substring(14);
                        }
                    }

                    userDataList.Add(userData);
                }
            }
        }

        return userDataList;
    }

    private DateTime? ParseDateTimeFromPart(string part, string prefix)
    {
        string timestamp = part.Substring(prefix.Length).Trim();
        if (!string.IsNullOrEmpty(timestamp))
        {
            if (DateTime.TryParseExact(timestamp, "yyyy-MM-dd-HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }
        }
        return null;
    }

    public class UserData
    {
        public string? ID { get; set; }
        public DateTime? Time { get; set; }
        public DateTime? NearestOnlineTime { get; set; }
        public string? WasUserOnline { get; set; }
    }
}