export async function findSerbianLocationCoordinates(location) {
  const normalizedLocation = location.trim().toLowerCase();

  const cacheKey = `carshare-location-${normalizedLocation}`;

  const cachedLocation = sessionStorage.getItem(cacheKey);

  if (cachedLocation) {
    return JSON.parse(cachedLocation);
  }

  const query = location.toLowerCase().includes("srb")
    ? location
    : `${location}, Srbija`;

  const parameters = new URLSearchParams({
    q: query,
    format: "jsonv2",
    countrycodes: "rs",
    limit: "1",
    "accept-language": "sr",
  });

  const response = await fetch(
    `https://nominatim.openstreetmap.org/search?${parameters}`,
  );

  if (!response.ok) {
    throw new Error("Servis za pronalaženje lokacije trenutno nije dostupan.");
  }

  const results = await response.json();

  if (results.length === 0) {
    return null;
  }

  const coordinates = {
    latitude: Number(results[0].lat),
    longitude: Number(results[0].lon),
    displayName: results[0].display_name,
  };

  sessionStorage.setItem(cacheKey, JSON.stringify(coordinates));

  return coordinates;
}
