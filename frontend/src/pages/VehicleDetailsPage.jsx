import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import {
  ArrowLeft,
  ChevronLeft,
  ChevronRight,
  Fuel,
  Gauge,
  MapPin,
  Maximize2,
  Settings2,
  ShieldCheck,
  Users,
  X,
} from "lucide-react";

import api from "../api/api";
import ReservationForm from "../components/ReservationForm";
import VehicleAvailabilityCalendar from "../components/VehicleAvailabilityCalendar";
import "./VehicleGallery.css";

const fallbackImage =
  "https://images.unsplash.com/photo-1503376780353-7e6692767b70?auto=format&fit=crop&w=1600&q=90";

const fuelNames = {
  0: "Benzin",
  1: "Dizel",
  2: "Električno",
  3: "Hibrid",
};

const transmissionNames = {
  0: "Manuelni",
  1: "Automatski",
};

function VehicleDetailsPage() {
  const { vehicleId } = useParams();
  const navigate = useNavigate();

  const [vehicle, setVehicle] = useState(null);
  const [activePhotoIndex, setActivePhotoIndex] =
    useState(0);
  const [isLightboxOpen, setIsLightboxOpen] =
    useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const loadVehicle = async () => {
      try {
        setError("");

        const response = await api.get(
          `/vehicles/${vehicleId}`
        );

        setVehicle(response.data);
        setActivePhotoIndex(0);
      } catch (requestError) {
        setError(
          requestError.response?.data?.detail ||
            "Vozilo nije pronađeno."
        );
      } finally {
        setIsLoading(false);
      }
    };

    loadVehicle();
  }, [vehicleId]);

  const validPhotos =
    vehicle?.photoUrls?.filter(
      (url) =>
        url &&
        !url.includes("example.com")
    ) ?? [];

  const galleryPhotos =
    validPhotos.length > 0
      ? validPhotos
      : [fallbackImage];

  const activePhoto =
    galleryPhotos[activePhotoIndex] ??
    galleryPhotos[0];

  const showPreviousPhoto = () => {
    setActivePhotoIndex((currentIndex) =>
      currentIndex === 0
        ? galleryPhotos.length - 1
        : currentIndex - 1
    );
  };

  const showNextPhoto = () => {
    setActivePhotoIndex((currentIndex) =>
      currentIndex === galleryPhotos.length - 1
        ? 0
        : currentIndex + 1
    );
  };

  if (isLoading) {
    return (
      <div className="details-status">
        Učitavanje vozila...
      </div>
    );
  }

  if (error || !vehicle) {
    return (
      <div className="details-status">
        <p>{error}</p>

        <button
          type="button"
          onClick={() => navigate("/vehicles")}
        >
          Nazad na vozila
        </button>
      </div>
    );
  }

  return (
    <div className="vehicle-details-page">
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
          Nazad na vozila
        </button>
      </header>

      <main className="details-container">
        <section className="vehicle-gallery">
          <div className="vehicle-gallery-main">
            <img
              src={activePhoto}
              alt={`${vehicle.brand} ${vehicle.model}`}
              onError={(event) => {
                event.currentTarget.onerror = null;
                event.currentTarget.src = fallbackImage;
              }}
            />

            <span className="details-verified">
              <ShieldCheck size={17} />
              Verifikovano vozilo
            </span>

            <button
              type="button"
              className="gallery-expand-button"
              onClick={() => setIsLightboxOpen(true)}
              aria-label="Uvećaj fotografiju"
            >
              <Maximize2 size={20} />
            </button>

            {galleryPhotos.length > 1 && (
              <>
                <button
                  type="button"
                  className="gallery-arrow gallery-arrow-left"
                  onClick={showPreviousPhoto}
                  aria-label="Prethodna fotografija"
                >
                  <ChevronLeft size={25} />
                </button>

                <button
                  type="button"
                  className="gallery-arrow gallery-arrow-right"
                  onClick={showNextPhoto}
                  aria-label="Sledeća fotografija"
                >
                  <ChevronRight size={25} />
                </button>

                <span className="gallery-counter">
                  {activePhotoIndex + 1} /{" "}
                  {galleryPhotos.length}
                </span>
              </>
            )}
          </div>

          {galleryPhotos.length > 1 && (
            <div className="vehicle-gallery-thumbnails">
              {galleryPhotos.map((photo, index) => (
                <button
                  type="button"
                  className={
                    index === activePhotoIndex
                      ? "gallery-thumbnail active"
                      : "gallery-thumbnail"
                  }
                  onClick={() =>
                    setActivePhotoIndex(index)
                  }
                  key={`${photo}-${index}`}
                >
                  <img
                    src={photo}
                    alt={`Fotografija ${index + 1}`}
                    onError={(event) => {
                      event.currentTarget.onerror = null;
                      event.currentTarget.src =
                        fallbackImage;
                    }}
                  />
                </button>
              ))}
            </div>
          )}
        </section>

        <section className="details-layout">
          <div className="details-main">
            <p className="section-label">
              DOSTUPNO ZA IZNAJMLJIVANJE
            </p>

            <div className="details-title">
              <div>
                <h1>
                  {vehicle.brand} {vehicle.model}
                </h1>

                <p>{vehicle.year}. godište</p>
              </div>

              <div className="details-price">
                <strong>
                  {vehicle.pricePerDay ?? "—"} €
                </strong>
                <span>po danu</span>
              </div>
            </div>

            <div className="details-location">
              <MapPin size={18} />
              {vehicle.address}
            </div>

            <div className="details-specifications">
              <div>
                <Users size={22} />
                <span>Sedišta</span>
                <strong>{vehicle.seatCount}</strong>
              </div>

              <div>
                <Gauge size={22} />
                <span>Kilometraža</span>
                <strong>
                  {Number(
                    vehicle.mileage ?? 0
                  ).toLocaleString()}{" "}
                  km
                </strong>
              </div>

              <div>
                <Settings2 size={22} />
                <span>Menjač</span>
                <strong>
                  {transmissionNames[
                    vehicle.transmissionType
                  ] ?? "Nepoznato"}
                </strong>
              </div>

              <div>
                <Fuel size={22} />
                <span>Gorivo</span>
                <strong>
                  {fuelNames[vehicle.fuelType] ??
                    "Nepoznato"}
                </strong>
              </div>
            </div>

            <div className="details-description">
              <h2>O ovom vozilu</h2>
              <p>{vehicle.description}</p>
            </div>

            <VehicleAvailabilityCalendar
              vehicleId={vehicleId}
            />

            <div className="details-rules">
              <h2>Informacije o najmu</h2>

              <div>
                <span>Dnevno ograničenje</span>
                <strong>
                  {vehicle.dailyMileageLimit ?? "—"} km
                </strong>
              </div>

              <div>
                <span>Depozit</span>
                <strong>
                  {vehicle.depositAmount} €
                </strong>
              </div>

              <div>
                <span>Boja</span>
                <strong>{vehicle.color}</strong>
              </div>
            </div>
          </div>

          <ReservationForm vehicle={vehicle} />
        </section>
      </main>

      {isLightboxOpen && (
        <div
          className="vehicle-lightbox"
          role="dialog"
          aria-modal="true"
          aria-label="Pregled fotografije vozila"
        >
          <button
            type="button"
            className="lightbox-close-button"
            onClick={() => setIsLightboxOpen(false)}
            aria-label="Zatvori"
          >
            <X size={27} />
          </button>

          {galleryPhotos.length > 1 && (
            <button
              type="button"
              className="lightbox-arrow lightbox-left"
              onClick={showPreviousPhoto}
              aria-label="Prethodna fotografija"
            >
              <ChevronLeft size={34} />
            </button>
          )}

          <img
            src={activePhoto}
            alt={`${vehicle.brand} ${vehicle.model}`}
            onError={(event) => {
              event.currentTarget.onerror = null;
              event.currentTarget.src = fallbackImage;
            }}
          />

          {galleryPhotos.length > 1 && (
            <button
              type="button"
              className="lightbox-arrow lightbox-right"
              onClick={showNextPhoto}
              aria-label="Sledeća fotografija"
            >
              <ChevronRight size={34} />
            </button>
          )}

          <span className="lightbox-counter">
            {activePhotoIndex + 1} /{" "}
            {galleryPhotos.length}
          </span>
        </div>
      )}
    </div>
  );
}

export default VehicleDetailsPage;