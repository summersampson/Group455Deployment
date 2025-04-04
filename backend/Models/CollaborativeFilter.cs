namespace NewsRecommender.Models
{
    public class CollaborativeFilter
    {
        public long ContentId { get; set; }
        public List<long> RecommendedIds { get; set; } = new();
    }
}
