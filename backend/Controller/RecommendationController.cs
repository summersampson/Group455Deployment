using Microsoft.AspNetCore.Mvc;
using NewsRecommender.Services;

namespace NewsRecommender.Controllers
{
    [ApiController]
    [Route("api/recommendations")]
    public class RecommendationController : ControllerBase
    {
        private readonly RecommendationService _recommendationService;

        public RecommendationController(RecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        [HttpGet("{contentId}")]
        public IActionResult GetRecommendations(long contentId)
        {
            var recommendations = _recommendationService.GetRecommendations(contentId);
            if (recommendations == null || recommendations.Count == 0)
                return NotFound(new { message = "No recommendations found." });

            return Ok(new { ContentId = contentId, Recommendations = recommendations });
        }
    }
}

