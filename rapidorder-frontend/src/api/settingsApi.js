const API_BASE = process.env.REACT_APP_API_BASE_URL || "http://localhost:5253";

export async function getTrackServedMission() {
  const response = await fetch(`${API_BASE}/api/settings/track-served-mission`);
  if (!response.ok) {
    throw new Error(`Failed to load track-served-mission setting: ${response.status}`);
  }
  return response.json();
}

export async function setTrackServedMission(enabled) {
  const response = await fetch(`${API_BASE}/api/settings/track-served-mission`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify({ trackServedMission: enabled })
  });

  if (!response.ok) {
    throw new Error(`Failed to update track-served-mission setting: ${response.status}`);
  }
}
