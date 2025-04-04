import { useState, FormEvent } from "react";

interface Recommendations {
  collaborative: string[];
  content: string[];
  azure: number[];
}

function RecommendationForm() {
  const [contentId, setContentId] = useState("");
  const [recommendations, setRecommendations] =
    useState<Recommendations | null>(null);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();

    const res = await fetch("http://localhost:5112/api/recommendations", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ contentId }),
    });

    const data = await res.json();
    setRecommendations(data);
  };

  return (
    <div>
      <h1>News Recommendations</h1>
      <form onSubmit={handleSubmit}>
        <input
          type="text"
          value={contentId}
          onChange={(e) => setContentId(e.target.value)}
          placeholder="Enter contentId"
          required
        />
        <button type="submit">Get Recommendations</button>
      </form>

      {recommendations && (
        <div>
          <h2>Recommendations for {contentId}</h2>

          <h3>Collaborative Filtering</h3>
          <ul>
            {recommendations.collaborative.map((title, i) => (
              <li key={i}>{title}</li>
            ))}
          </ul>

          <h3>Content-Based Filtering</h3>
          <ul>
            {recommendations.content.map((id, i) => (
              <li key={i}>{id}</li>
            ))}
          </ul>

          <h3>Azure ML</h3>
          <ul>
            {recommendations.azure.map((id) => (
              <li key={id}>{id}</li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
}

export default RecommendationForm;
