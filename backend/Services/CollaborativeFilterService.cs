using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using NewsRecommender.Models;

namespace NewsRecommender.Services
{
    public class RecommendationService
    {
        private readonly Dictionary<long, List<string>> _recommendations;

        public RecommendationService(IWebHostEnvironment env)
        {
            var filePath = Path.Combine(env.ContentRootPath, "Data", "collaborative_filtering.csv");
            _recommendations = LoadRecommendations(filePath);
        }

        private Dictionary<long, List<string>> LoadRecommendations(string filePath)
        {
            var recommendations = new Dictionary<long, List<string>>();

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"CSV file not found: {filePath}");
                return recommendations;
            }

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            }))
            {
                while (csv.Read())
                {
                    try
                    {
                        var contentId = csv.GetField<long>("contentId");
                        var recommendedTitles = new List<string>();

                        for (int i = 2; i <= 6; i++) // Columns "Recommendation 1" to "Recommendation 5"
                        {
                            var columnName = csv.HeaderRecord[i];
                            var recommendation = csv.GetField<string>(columnName);

                            if (!string.IsNullOrWhiteSpace(recommendation))
                            {
                                recommendedTitles.Add(recommendation);
                            }
                        }

                        recommendations[contentId] = recommendedTitles;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading CSV row: {ex.Message}");
                    }
                }
            }

            return recommendations;
        }

        public List<string>? GetRecommendations(long contentId)
        {
            return _recommendations.TryGetValue(contentId, out var recs) ? recs : null;
        }
    }
}
