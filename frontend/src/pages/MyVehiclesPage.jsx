import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  ArrowLeft,
  CalendarDays,
  Car,
  Eye,
  MapPin,
  Pencil,
  Plus,
  ShieldCheck,
  Trash2,
} from "lucide-react";

import api from "../api/api";
import VehicleVerificationForm from "../components/VehicleVerificationForm";

const verificationStatuses = {
  0: {
    text: "Dokument nije poslat",
    className: "not-submitted",
  },
  1: {
    text: "Čeka verifikaciju",
    className: "pending",
  },
  2: {
    text: "Verifikovano",
    className: "approved",
  },
  3: {
    text: "Odbijeno",
    className: "rejected",
  },
};

const fallbackImages = [
  "https://images.unsplash.com/photo-1492144534655-ae79c964c9d7?auto=format&fit=crop&w=900&q=85",
  "https://images.unsplash.com/photo-1549317661-bd32c8ce0db2?auto=format&fit=crop&w=900&q=85",
  "https://images.unsplash.com/photo-1503376780353-7e6692767b70?auto=format&fit=crop&w=900&q=85",
];

function MyVehiclesPage() {
  const navigate = useNavigate();

  const [vehicles, setVehicles] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");

  const loadMyVehicles = async () => {
    try {
      setError("");

      const [profileResponse, vehiclesResponse] =
        await Promise.all([
          api.get("/users/me"),
          api.get("/vehicles"),
        ]);

      const currentUserId = profileResponse.data.id;

      setVehicles(
        vehiclesResponse.data.filter(
          (vehicle) =>
            vehicle.ownerId === currentUserId
        )
      );
    } catch (requestError) {
      setError(
        requestError.response?.data?.detail ||
          "Nije moguće učitati vaša vozila."
      );
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadMyVehicles();
  }, []);

  const deleteVehicle = async (vehicleId) => {
    const confirmed = window.confirm(
      "Da li sigurno želite da obrišete ovo vozilo?"
    );

    if (!confirmed) {
      return;
    }

    try {
      await api.delete(`/vehicles/${vehicleId}`);

      setVehicles((currentVehicles) =>
        currentVehicles.filter(
          (vehicle) => vehicle.id !== vehicleId
        )
      );
    } catch (requestError) {
      window.alert(
        requestError.response?.data?.detail ||
          "Vozilo nije moguće obrisati."
      );
    }
  };

  return (
    <div className="my-vehicles-page">
      <header className="details-header">
        <button
          type="button"
          className="brand"
          onClick={() => navigate("/vehicles")}
        >
          <span className="brand-mark">◆</span>
          <span>CarShare</span>
        </button>

        <button
          type="button"
          className="back-button"
          onClick={() => navigate("/vehicles")}
        >
          <ArrowLeft size={18} />
          Nazad na ponudu
        </button>
      </header>

      <main className="my-vehicles-container">
        <div className="my-vehicles-heading">
          <div>
            <p className="section-label">
              VLASNIČKI PANEL
            </p>

            <h1>Moja vozila</h1>

            <p>
              Upravljajte vozilima, dostupnošću i statusom
              verifikacije.
            </p>
          </div>

          <button
            type="button"
            onClick={() =>
              navigate("/vehicles/create")
            }
          >
            <Plus size={18} />
            Dodaj vozilo
          </button>
        </div>

        {isLoading && (
          <div className="status-card">
            Učitavanje vozila...
          </div>
        )}

        {error && (
          <div className="status-card error-message">
            {error}
          </div>
        )}

        {!isLoading &&
          !error &&
          vehicles.length === 0 && (
            <div className="empty-vehicles">
              <Car size={45} />

              <h2>Još nemate vozila</h2>

              <p>
                Postavite prvo vozilo i počnite da primate
                rezervacije.
              </p>

              <button
                type="button"
                onClick={() =>
                  navigate("/vehicles/create")
                }
              >
                Postavi vozilo
              </button>
            </div>
          )}

        <div className="my-vehicles-grid">
          {vehicles.map((vehicle, index) => {
            const verification =
              verificationStatuses[
                vehicle.verificationStatus
              ] ?? verificationStatuses[0];

            const suppliedPhoto =
              vehicle.photoUrls?.[0];

            const image =
              suppliedPhoto &&
              !suppliedPhoto.includes("example.com")
                ? suppliedPhoto
                : fallbackImages[
                    index % fallbackImages.length
                  ];

            return (
              <article
                className="my-vehicle-card"
                key={vehicle.id}
              >
                <img
                  src={image}
                  alt={`${vehicle.brand} ${vehicle.model}`}
                />

                <div className="my-vehicle-content">
                  <span
                    className={`verification-badge ${verification.className}`}
                  >
                    <ShieldCheck size={14} />
                    {verification.text}
                  </span>

                  <h2>
                    {vehicle.brand} {vehicle.model}
                  </h2>

                  <p>{vehicle.year}. godište</p>

                  <span className="my-vehicle-location">
                    <MapPin size={15} />
                    {vehicle.address}
                  </span>

                  <div className="my-vehicle-price">
                    <span>
                      Cena
                      <strong>
                        {vehicle.pricePerDay ?? "—"} € / dan
                      </strong>
                    </span>

                    <span>
                      Depozit
                      <strong>
                        {vehicle.depositAmount} €
                      </strong>
                    </span>
                  </div>

                  {(vehicle.verificationStatus === 0 ||
                    vehicle.verificationStatus === 3) && (
                    <VehicleVerificationForm
                      vehicleId={vehicle.id}
                      onSubmitted={() =>
                        setVehicles((currentVehicles) =>
                          currentVehicles.map(
                            (currentVehicle) =>
                              currentVehicle.id ===
                              vehicle.id
                                ? {
                                    ...currentVehicle,
                                    verificationStatus: 1,
                                  }
                                : currentVehicle
                          )
                        )
                      }
                    />
                  )}

                  <button
                    type="button"
                    className="manage-vehicle-button"
                    onClick={() =>
                      navigate(
                        `/vehicles/${vehicle.id}`
                      )
                    }
                  >
                    <Eye size={17} />
                    Pogledaj detalje
                  </button>

                  <button
                    type="button"
                    className="manage-vehicle-button"
                    onClick={() =>
                      navigate(
                        `/owner/vehicles/${vehicle.id}/edit`
                      )
                    }
                  >
                    <Pencil size={17} />
                    Izmeni vozilo
                  </button>

                  <button
                    type="button"
                    className="manage-availability-button"
                    onClick={() =>
                      navigate(
                        `/owner/vehicles/${vehicle.id}/availability`
                      )
                    }
                  >
                    <CalendarDays size={17} />
                    Upravljaj dostupnošću
                  </button>

                  <button
                    type="button"
                    className="delete-vehicle-button"
                    onClick={() =>
                      deleteVehicle(vehicle.id)
                    }
                  >
                    <Trash2 size={17} />
                    Obriši vozilo
                  </button>
                </div>
              </article>
            );
          })}
        </div>
      </main>
    </div>
  );
}

export default MyVehiclesPage;