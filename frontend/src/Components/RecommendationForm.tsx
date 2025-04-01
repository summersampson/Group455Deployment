import { useState, FormEvent } from "react";

interface Recommendations {
  collaborative: number[];
  content: number[];
  azure: number[];
}

function RecommendationForm() {
  const [userId, setUserId] = useState("");
  const [recommendations, setRecommendations] =
    useState<Recommendations | null>(null);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();

    const res = await fetch("http://localhost:5000/api/recommendations", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ userId }),
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
          value={userId}
          onChange={(e) => setUserId(e.target.value)}
          placeholder="Enter User ID"
          required
        />
        <button type="submit">Get Recommendations</button>
      </form>

      {recommendations && (
        <div>
          <h2>Recommendations for {userId}</h2>

          <h3>Collaborative Filtering</h3>
          <ul>
            {recommendations.collaborative.map((id) => (
              <li key={id}>{id}</li>
            ))}
          </ul>

          <h3>Content-Based Filtering</h3>
          <ul>
            {recommendations.content.map((id) => (
              <li key={id}>{id}</li>
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
