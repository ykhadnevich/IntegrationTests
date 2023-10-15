using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using static PredictionController;

[Route("api/")]
[ApiController]
public class PredictionController : ControllerBase
{
    private List<UserRecord> UserDataList;

    public PredictionController()
    {
        string filePath = "C:\\Users\\user\\source\\repos\\TDD\\TDD2\\Data2.txt";
        UserDataList = ParseTextFile1(filePath);
    }

    [HttpGet("predictions/user")]
    public IActionResult PredictUserOnline([FromQuery] string date, [FromQuery] double tolerance, [FromQuery] Guid userId)
    {
        if (UserDataList.Count == 0)
        {
            return NotFound("No historical data available.");
        }

        if (DateTime.TryParseExact(date, "yyyy-dd-MM-HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime targetDate))
        {
            // Filter historical data to include only data for the same weekday and time
            var userRecords = UserDataList
                .Where(record => record.UserId == userId && record.Time.DayOfWeek == targetDate.DayOfWeek && record.Time.Hour == targetDate.Hour)
                .ToList();

            if (userRecords.Count > 0)
            {
                // Calculate the chance based on the specified formula

                double wasOnlineAtThisWeekdayAtThisTime = userRecords.Count(record => record.WasUserOnline == "online");
                double totalWeeks = userRecords.Count;
                double onlineChance = wasOnlineAtThisWeekdayAtThisTime / totalWeeks;

                // Determine if the user will be online based on the tolerance
                bool willBeOnline = onlineChance > tolerance;

                return Ok(new { willBeOnline, onlineChance });
            }
            else
            {
                return NotFound("No historical data available for the specified user, weekday, and time.");
            }
        }

        return BadRequest("Invalid date format.");
    }






    private List<UserRecord> ParseTextFile1(string filePath)
    {
        List<UserRecord> userDataList = new List<UserRecord>();

        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split(';');
                if (parts.Length == 4)
                {
                    UserRecord userData = new UserRecord();

                    foreach (string part in parts)
                    {
                        if (part.StartsWith("ID:"))
                        {
                            userData.UserId = Guid.Parse(parts[0].Split(':')[1].Trim());
                        }
                        else if (part.StartsWith("Time:"))
                        {
                            userData.Time = DateTime.ParseExact(part.Substring(5), "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
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
            if (DateTime.TryParseExact(timestamp, /*"dd.MM.yyyy HH:mm:ss"*/ "yyyy-MM-dd-HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }
        }
        return null;
    }

    public class UserRecord
    {
        public Guid UserId { get; set; }
        public DateTime Time { get; set; }
        public DateTime NearestOnlineTime { get; set; }
        public string? WasUserOnline { get; set; }
    }
}