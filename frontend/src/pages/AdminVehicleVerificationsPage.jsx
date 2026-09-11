import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  ArrowLeft,
  CalendarDays,
  Check,
  ExternalLink,
  Mail,
  ShieldCheck,
  User,
  X,
} from "lucide-react";

import api from "../api/api";

function formatDate(date) {
  if (!date) {
    return "Datum nije dostupan";
  }

  return new Intl.DateTimeFormat("sr-RS", {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(new Date(date));
}

function AdminVehicleVerificationsPage() {
  const navigate = useNavigate();

  const [vehicles, setVehicles] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");

  const loadVehicles = async () => {
    try {
      setError("");

      const response = await api.get(
        "/admin/vehicle-verifications/pending"
      );

      setVehicles(response.data);
    } catch (error) {
      setError(
        error.response?.status === 403
          ? "Ova stranica je dostupna samo administratoru."
          : "Nije moguće učitati vozila za verifikaciju."
      );
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadVehicles();
  }, []);

  const processVehicle = async (
    vehicleId,
    action
  ) => {
    const actionText =
      action === "approve" ? "odobrite" : "odbijete";

    const confirmed = window.confirm(
      `Da li želite da ${actionText} ovo vozilo?`
    );

    if (!confirmed) {
      return;
    }

    try {
      await api.patch(
        `/admin/vehicle-verifications/${vehicleId}/${action}`
      );

      setVehicles((currentVehicles) =>
        currentVehicles.filter(
          (vehicle) => vehicle.vehicleId !== vehicleId
        )
      );
    } catch (error) {
      window.alert(
        error.response?.data?.detail ??
          "Vozilo nije moguće obraditi."
      );
    }
  };

  return (
    <div className="admin-page">
      <header className="details-header">
        <button
          className="brand"
          onClick={() => navigate("/vehicles")}
        >
          <span className="brand-mark">◆</span>
          <span>CarShare</span>
        </button>

        <button
          className="back-button"
          onClick={() => navigate("/vehicles")}
        >
          <ArrowLeft size={18} />
          Nazad
        </button>
      </header>

      <main className="admin-container">
        <div className="admin-heading">
          <div>
            <p className="section-label">
              ADMINISTRATORSKI PANEL
            </p>

            <h1>Verifikacija vozila</h1>

            <p>
              Pregledajte dokumentaciju i odobrite vozila
              koja ispunjavaju uslove.
            </p>
          </div>

          <div className="admin-counter">
            <ShieldCheck size={22} />
            <span>
              <strong>{vehicles.length}</strong>
              zahteva na čekanju
            </span>
          </div>
        </div>

        {isLoading && (
          <div className="status-card">
            Učitavanje zahteva...
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
            <div className="admin-empty">
              <ShieldCheck size={48} />
              <h2>Svi zahtevi su obrađeni</h2>
              <p>
                Trenutno nema vozila koja čekaju
                verifikaciju.
              </p>
            </div>
          )}

        <div className="admin-verification-grid">
          {vehicles.map((vehicle) => (
            <article
              className="verification-request-card"
              key={vehicle.vehicleId}
            >
              <div className="verification-request-top">
                <span>Čeka proveru</span>

                <strong>
                  {vehicle.brand} {vehicle.model}
                </strong>

                <small>{vehicle.year}. godište</small>
              </div>

              <div className="verification-owner">
                <div>
                  {vehicle.ownerName
                    ?.charAt(0)
                    .toUpperCase()}
                </div>

                <span>
                  <strong>{vehicle.ownerName}</strong>

                  <small>
                    <Mail size={13} />
                    {vehicle.ownerEmail}
                  </small>
                </span>
              </div>

              <div className="verification-information">
                <div>
                  <User size={17} />

                  <span>
                    <small>ID vlasnika</small>
                    {vehicle.ownerId}
                  </span>
                </div>

                <div>
                  <CalendarDays size={17} />

                  <span>
                    <small>Poslato</small>
                    {formatDate(vehicle.submittedAtUtc)}
                  </span>
                </div>
              </div>

              <a
                className="document-link"
                href={vehicle.registrationDocumentUrl}
                target="_blank"
                rel="noreferrer"
              >
                <ExternalLink size={17} />
                Otvori registracioni dokument
              </a>

              <div className="verification-actions">
                <button
                  className="reject-button"
                  onClick={() =>
                    processVehicle(
                      vehicle.vehicleId,
                      "reject"
                    )
                  }
                >
                  <X size={18} />
                  Odbij
                </button>

                <button
                  className="approve-button"
                  onClick={() =>
                    processVehicle(
                      vehicle.vehicleId,
                      "approve"
                    )
                  }
                >
                  <Check size={18} />
                  Odobri
                </button>
              </div>
            </article>
          ))}
        </div>
      </main>
    </div>
  );
}

export default AdminVehicleVerificationsPage;