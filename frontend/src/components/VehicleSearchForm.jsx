import { useState } from "react";
import {
  CalendarDays,
  List,
  MapPin,
  Search,
} from "lucide-react";

import api from "../api/api";

function formatForDateTimeInput(date) {
  const timezoneOffset = date.getTimezoneOffset() * 60000;

  return new Date(date.getTime() - timezoneOffset)
    .toISOString()
    .slice(0, 16);
}

function createFutureDate(daysFromToday) {
  const date = new Date();

  date.setDate(date.getDate() + daysFromToday);
  date.setHours(10, 0, 0, 0);

  return formatForDateTimeInput(date);
}

async function findLocationCoordinates(location) {
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
    `https://nominatim.openstreetmap.org/search?${parameters}`
  );

  if (!response.ok) {
    throw new Error("Geocoding service nije dostupan.");
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

  sessionStorage.setItem(
    cacheKey,
    JSON.stringify(coordinates)
  );

  return coordinates;
}

export default function VehicleSearchForm({
  onResults,
  onError,
}) {
  const [location, setLocation] = useState("Novi Pazar");
  const [startDateTime, setStartDateTime] = useState(
    createFutureDate(1)
  );
  const [endDateTime, setEndDateTime] = useState(
    createFutureDate(2)
  );
  const [isSearching, setIsSearching] = useState(false);

  const minimumDateTime = formatForDateTimeInput(new Date());

  const handleSubmit = async (event) => {
    event.preventDefault();

    if (!location.trim()) {
      onError("Unesite grad ili adresu.");
      return;
    }

    const startDate = new Date(startDateTime);
    const endDate = new Date(endDateTime);
    const currentDate = new Date();

    if (startDate <= currentDate) {
      onError(
        "Datum preuzimanja mora biti u budućnosti."
      );
      return;
    }

    if (startDate >= endDate) {
      onError(
        "Datum vraćanja mora biti posle datuma preuzimanja."
      );
      return;
    }

    try {
      setIsSearching(true);
      onError("");

      const coordinates =
        await findLocationCoordinates(location);

      if (!coordinates) {
        onError(
          "Lokacija nije pronađena. Proverite naziv grada ili adresu."
        );
        return;
      }

      const response = await api.get("/vehicles/search", {
        params: {
          startDateTimeUtc: startDate.toISOString(),
          endDateTimeUtc: endDate.toISOString(),
          latitude: coordinates.latitude,
          longitude: coordinates.longitude,
          radiusKilometers: 200,
        },
      });

      onResults(
        response.data.filter(
          (vehicle) => vehicle.verificationStatus === 2
        )
      );
    } catch (requestError) {
      onError(
        requestError.response?.data?.detail ||
          requestError.message ||
          "Nije moguće izvršiti pretragu."
      );
    } finally {
      setIsSearching(false);
    }
  };

  const handleShowAll = async () => {
    try {
      setIsSearching(true);
      onError("");

      const response = await api.get("/vehicles");

      onResults(
        response.data.filter(
          (vehicle) => vehicle.verificationStatus === 2
        )
      );
    } catch (requestError) {
      onError(
        requestError.response?.data?.detail ||
          "Nije moguće učitati vozila."
      );
    } finally {
      setIsSearching(false);
    }
  };

  return (
    <form className="search-panel" onSubmit={handleSubmit}>
      <label className="search-field location-search-field">
        <MapPin size={20} />

        <div>
          <span>Grad ili adresa</span>

          <input
            type="text"
            value={location}
            placeholder="Na primer: Tutin"
            onChange={(event) =>
              setLocation(event.target.value)
            }
          />

          <small className="map-attribution">
            Pretraga lokacije: OpenStreetMap
          </small>
        </div>
      </label>

      <label className="search-field">
        <CalendarDays size={20} />

        <div>
          <span>Preuzimanje</span>

          <input
            type="datetime-local"
            min={minimumDateTime}
            value={startDateTime}
            onChange={(event) =>
              setStartDateTime(event.target.value)
            }
          />
        </div>
      </label>

      <label className="search-field">
        <CalendarDays size={20} />

        <div>
          <span>Vraćanje</span>

          <input
            type="datetime-local"
            min={startDateTime}
            value={endDateTime}
            onChange={(event) =>
              setEndDateTime(event.target.value)
            }
          />
        </div>
      </label>

      <button
        className="search-button"
        type="submit"
        disabled={isSearching}
      >
        <Search size={19} />
        {isSearching ? "Tražim..." : "Pretraži"}
      </button>

      <button
        className="show-all-vehicles-button"
        type="button"
        disabled={isSearching}
        onClick={handleShowAll}
      >
        <List size={18} />
        Sva vozila
      </button>
    </form>
  );
}