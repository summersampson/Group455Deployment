using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Globalization;
using CsvHelper;
using System.IO;

[ApiController]
[Route("api/[controller]")]
public class RecommendationsController : ControllerBase
{
    [HttpPost]
    public IActionResult Post([FromBody] RecommendationRequest request)
    {
        var contentId = request.ContentId;

        var contentRecs = GetTop5("Data/content_similarity_matrix.csv", contentId);
        var collaborativeRecs = GetCollaborativeTitles("Data/collaborative_filtering.csv", contentId);
        var azureRecs = new List<long>(); // Optional for now

        var result = new
        {
            collaborative = collaborativeRecs,  // List<string>
            content = contentRecs,              // List<long>
            azure = azureRecs                   // List<long>
        };

        return Ok(result);
    }

    // ✅ For content_similarity_matrix.csv
    private List<long> GetTop5(string csvPath, long contentId)
    {
        var table = new DataTable();

        using (var reader = new StreamReader(csvPath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        using (var dr = new CsvDataReader(csv))
        {
            table.Load(dr);
        }

        if (!table.Columns.Contains(contentId.ToString()))
            return new List<long>();

        var row = table.AsEnumerable()
                       .FirstOrDefault(r => r.Field<string>(0) == contentId.ToString());

        if (row == null)
            return new List<long>();

        var scores = row.ItemArray
                        .Skip(1)
                        .Select((val, index) => new
                        {
                            Index = index + 1,
                            Score = Convert.ToDouble(val)
                        })
                        .OrderByDescending(x => x.Score)
                        .Take(6)  // Include self
                        .Skip(1)  // Skip self
                        .Select(x => long.Parse(table.Columns[x.Index].ColumnName))
                        .ToList();

        return scores;
    }

    // ✅ For collaborative_filtering.csv
    private List<string> GetCollaborativeTitles(string csvPath, long contentId)
    {
        if (!System.IO.File.Exists(csvPath))
            return new List<string>();

        using (var reader = new StreamReader(csvPath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csv.GetRecords<dynamic>();

            foreach (var record in records)
            {
                if (long.TryParse(record.contentId, out long id) && id == contentId)
                {
                    var titles = new List<string>();
                    for (int i = 1; i <= 5; i++)
                    {
                        var title = (string)record[$"Recommendation {i}"];
                        if (!string.IsNullOrWhiteSpace(title))
                        {
                            titles.Add(title);
                        }
                    }
                    return titles;
                }
            }
        }

        return new List<string>();
    }

    public class RecommendationRequest
    {
        public long ContentId { get; set; }
    }
}
