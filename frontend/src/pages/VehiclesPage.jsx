import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  CalendarDays,
  Gauge,
  Heart,
  LogOut,
  MapPin,
  ShieldCheck,
  Users,
} from "lucide-react";

import api from "../api/api";
import VehicleSearchForm from "../components/VehicleSearchForm";

const fallbackImages = [
  "https://images.unsplash.com/photo-1503376780353-7e6692767b70?auto=format&fit=crop&w=900&q=85",
  "https://images.unsplash.com/photo-1492144534655-ae79c964c9d7?auto=format&fit=crop&w=900&q=85",
  "https://images.unsplash.com/photo-1549317661-bd32c8ce0db2?auto=format&fit=crop&w=900&q=85",
];

function getVehicleImage(vehicle, index) {
  const photo = vehicle.photoUrls?.[0];

  if (photo && !photo.includes("example.com")) {
    return photo;
  }

  return fallbackImages[index % fallbackImages.length];
}

function VehiclesPage() {
  const navigate = useNavigate();

  const [vehicles, setVehicles] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");
  const [profile, setProfile] = useState(null);

  const loadAllVehicles = async () => {
    try {
      setIsLoading(true);
      setError("");

      const [vehiclesResponse, profileResponse] =
        await Promise.all([
          api.get("/vehicles"),
          api.get("/users/me"),
        ]);

      setVehicles(
        vehiclesResponse.data.filter(
          (vehicle) =>
            vehicle.verificationStatus === 2
        )
      );

      setProfile(profileResponse.data);
    } catch (requestError) {
      setError(
        requestError.response?.data?.detail ||
          "Nije moguće učitati vozila."
      );
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadAllVehicles();
  }, []);

  const handleSearchResults = (results) => {
    setError("");
    setVehicles(results);

    setTimeout(() => {
      document
        .getElementById("vehicles")
        ?.scrollIntoView({
          behavior: "smooth",
        });
    }, 100);
  };

  const handleSearchError = (message) => {
    setVehicles([]);
    setError(
      message || "Pretragu nije moguće izvršiti."
    );
  };

  const logout = () => {
    localStorage.removeItem("accessToken");
    localStorage.removeItem("refreshToken");

    navigate("/login");
  };

  return (
    <div className="vehicles-page">
      <header className="main-header">
        <button
          type="button"
          className="brand"
          onClick={() => navigate("/vehicles")}
        >
          <span className="brand-mark">◆</span>
          <span>CarShare</span>
        </button>

        <nav className="main-navigation">
          <a href="#vehicles">Vozila</a>

          <a href="#how-it-works">
            Kako funkcioniše
          </a>

          <a href="#about">O nama</a>

          <button
            type="button"
            className="navigation-button"
            onClick={() =>
              navigate("/reservations")
            }
          >
            Moje rezervacije
          </button>

          {profile?.isOwner && (
            <>
              <button
                type="button"
                className="navigation-button"
                onClick={() =>
                  navigate("/owner/vehicles")
                }
              >
                Moja vozila
              </button>

              <button
                type="button"
                className="navigation-button"
                onClick={() =>
                  navigate("/owner/reservations")
                }
              >
                Zahtevi vlasnika
              </button>
            </>
          )}

          {profile?.isAdmin && (
            <button
              type="button"
              className="navigation-button"
              onClick={() =>
                navigate("/admin/statistics")
              }
            >
              Admin panel
            </button>
          )}

          <button
            type="button"
            className="navigation-button"
            onClick={() => navigate("/profile")}
          >
            Moj profil
          </button>
        </nav>

        <div className="header-actions">
          {profile?.isOwner && (
            <button
              type="button"
              className="host-button"
              onClick={() =>
                navigate("/vehicles/create")
              }
            >
              Postavi vozilo
            </button>
          )}

          <button
            type="button"
            className="logout-button"
            onClick={logout}
          >
            <LogOut size={17} />
            Odjavi se
          </button>
        </div>
      </header>

      <main>
        <section className="rental-hero">
          <div className="hero-overlay" />

          <div className="hero-content">
            <p className="hero-label">
              BOLJI NAČIN ZA PUTOVANJE
            </p>

            <h1>
              Pronađite svoju
              <br />
              sledeću vožnju
            </h1>

            <p className="hero-description">
              Pouzdani automobili od verifikovanih
              vlasnika. Fleksibilno iznajmljivanje. Više
              slobode.
            </p>
          </div>

          <VehicleSearchForm
            onResults={handleSearchResults}
            onError={handleSearchError}
          />

          {error && (
            <p className="search-error">{error}</p>
          )}

          <div className="hero-benefits">
            <div>
              <ShieldCheck size={22} />

              <span>
                <strong>Provereni vlasnici</strong>
                Verifikovani na CarShare platformi
              </span>
            </div>

            <div>
              <CalendarDays size={22} />

              <span>
                <strong>Fleksibilni termini</strong>
                Rezervišite prema svojim potrebama
              </span>
            </div>

            <div>
              <MapPin size={22} />

              <span>
                <strong>Lokalna vozila</strong>
                Automobili u vašoj blizini
              </span>
            </div>
          </div>
        </section>

        <section
          id="vehicles"
          className="vehicles-section"
        >
          <div className="section-heading">
            <div>
              <p className="section-label">
                IZDVOJENA VOZILA
              </p>

              <h2>
                Pronađite automobil po svojoj meri
              </h2>
            </div>

            <span className="vehicle-count">
              {vehicles.length} dostupnih vozila
            </span>
          </div>

          {isLoading && (
            <div className="status-card">
              Učitavanje vozila...
            </div>
          )}

          {!isLoading &&
            !error &&
            vehicles.length === 0 && (
              <div className="status-card">
                Trenutno nema dostupnih verifikovanih
                vozila.
              </div>
            )}

          {!isLoading && error && (
            <div className="status-card error-message">
              {error}
            </div>
          )}

          <div className="vehicle-grid">
            {vehicles.map((vehicle, index) => (
              <article
                className="vehicle-card"
                key={vehicle.id}
              >
                <div className="vehicle-photo">
                  <img
                    src={getVehicleImage(
                      vehicle,
                      index
                    )}
                    alt={`${vehicle.brand} ${vehicle.model}`}
                  />

                  <span className="availability-badge">
                    Dostupno
                  </span>

                  <button
                    type="button"
                    className="favorite-button"
                    aria-label="Dodaj u omiljena"
                  >
                    <Heart size={19} />
                  </button>
                </div>

                <div className="vehicle-details">
                  <div className="vehicle-title-row">
                    <div>
                      <h3>
                        {vehicle.brand}{" "}
                        {vehicle.model}
                      </h3>

                      <p>{vehicle.year}. godište</p>
                    </div>

                    <span className="rating">
                      ★ 4.9
                    </span>
                  </div>

                  <div className="vehicle-location">
                    <MapPin size={16} />
                    {vehicle.address}
                  </div>

                  <div className="vehicle-specifications">
                    <span>
                      <Users size={16} />
                      {vehicle.seatCount} sedišta
                    </span>

                    <span>
                      <Gauge size={16} />
                      {Number(
                        vehicle.mileage ?? 0
                      ).toLocaleString()}{" "}
                      km
                    </span>
                  </div>

                  <div className="vehicle-footer">
                    <div className="vehicle-price">
                      <strong>
                        {vehicle.pricePerDay ?? "—"} €
                      </strong>

                      <span>/ dan</span>
                    </div>

                    <button
                      type="button"
                      onClick={() =>
                        navigate(
                          `/vehicles/${vehicle.id}`
                        )
                      }
                    >
                      Pogledaj vozilo
                    </button>
                  </div>
                </div>
              </article>
            ))}
          </div>
        </section>
      </main>
    </div>
  );
}

export default VehiclesPage;