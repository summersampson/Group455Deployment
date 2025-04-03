namespace NewsRecommender.Models
{
    public class CollaborativeFilter
    {
        public long ContentId { get; set; }
        public List<string> RecommendedTitles { get; set; } = new();
    }
}
