using Microsoft.AspNetCore.Mvc;
using System.Globalization;

[Route("api/stats")]
[ApiController]
public class StatsController : ControllerBase
{
    public List<StatsData> StatsDataList { get; }

    public class StatsData
    {
        public DateTime DateTime { get; set; }
        public int UsersOnline { get; set; }
    }
    public StatsController()
    {
        string filePath = Path.Combine("C:\\Users\\user\\source\\repos\\TDD\\TDD\\Data.txt");
        StatsDataList = ParseStatsFile(filePath);
    }


    [HttpGet("users")]
    public IActionResult GetUsers([FromQuery] string date)
    {
        if (StatsDataList.Count == 0)
        {
            return NotFound();
        }

        if (DateTime.TryParseExact(date, "yyyy-MM-dd-HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
        {
            var data = StatsDataList.Find(data => data.DateTime == dateTime);
            if (data != null)
            {
                return Ok(new { usersOnline = data.UsersOnline });
            }
            return Ok(new { usersOnline = (object)null });
        }

        return BadRequest("Invalid date format.");
    }



    private List<StatsData> ParseStatsFile(string filePath)
    {
        List<StatsData> statsDataList = new List<StatsData>();

        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split(';');
                if (parts.Length == 2)
                {
                    if (DateTime.TryParseExact(parts[0].Trim(), "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime)
                        && int.TryParse(parts[1].Trim().Split(':')[1], out int usersOnline))
                    {
                        statsDataList.Add(new StatsData
                        {
                            DateTime = dateTime,
                            UsersOnline = usersOnline
                        });
                    }
                }
            }
        }

        return statsDataList;
    }

}