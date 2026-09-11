import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  ArrowLeft,
  CalendarDays,
  Check,
  Mail,
  User,
  X,
} from "lucide-react";

import api from "../api/api";
import HandoverForm from "../components/HandoverForm";
import DisputeForm from "../components/DisputeForm";

const statusData = {
  0: {
    text: "Na čekanju",
    className: "pending",
  },
  1: {
    text: "Odobrena",
    className: "approved",
  },
  2: {
    text: "Odbijena",
    className: "rejected",
  },
  3: {
    text: "U toku",
    className: "active",
  },
  4: {
    text: "Završena",
    className: "completed",
  },
  5: {
    text: "Otkazana",
    className: "cancelled",
  },
  6: {
    text: "Otvoren spor",
    className: "disputed",
  },
};

function formatDate(date) {
  return new Intl.DateTimeFormat("sr-RS", {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(new Date(date));
}

function OwnerReservationsPage() {
  const navigate = useNavigate();

  const [reservations, setReservations] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");

  const loadReservations = async () => {
    try {
      setError("");

      const response = await api.get(
        "/reservations/owner"
      );

      setReservations(response.data);
    } catch (requestError) {
      setError(
        requestError.response?.status === 403
          ? "Ova stranica je dostupna samo vlasnicima."
          : requestError.response?.data?.detail ||
              "Nije moguće učitati zahteve."
      );
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadReservations();
  }, []);

  const changeStatus = async (
    reservationId,
    action
  ) => {
    try {
      await api.patch(
        `/reservations/${reservationId}/${action}`
      );

      await loadReservations();
    } catch (requestError) {
      window.alert(
        requestError.response?.data?.detail ||
          "Status rezervacije nije moguće promeniti."
      );
    }
  };

  return (
    <div className="reservations-page">
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

      <main className="reservations-container">
        <div className="reservations-heading">
          <p className="section-label">
            ZAHTEVI ZA VAŠA VOZILA
          </p>

          <h1>Rezervacije vlasnika</h1>

          <p>
            Pregledajte zahteve, potvrdite primopredaju i
            pratite završene rezervacije.
          </p>
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
          reservations.length === 0 && (
            <div className="status-card">
              Trenutno nema zahteva za vaša vozila.
            </div>
          )}

        <div className="owner-reservations-grid">
          {reservations.map((reservation) => {
            const status =
              statusData[reservation.status] ??
              statusData[0];

            return (
              <article
                className="owner-reservation-card"
                key={reservation.id}
              >
                <div className="owner-reservation-heading">
                  <div>
                    <span
                      className={`reservation-status ${status.className}`}
                    >
                      {status.text}
                    </span>

                    <h2>
                      {reservation.vehicleBrand}{" "}
                      {reservation.vehicleModel}
                    </h2>
                  </div>

                  <strong>
                    {reservation.rentalAmount} €
                  </strong>
                </div>

                <div className="renter-information">
                  <div className="renter-avatar">
                    {reservation.renterName
                      ?.charAt(0)
                      .toUpperCase()}
                  </div>

                  <div>
                    <strong>
                      {reservation.renterName}
                    </strong>

                    <span>
                      <Mail size={14} />
                      {reservation.renterEmail}
                    </span>
                  </div>
                </div>

                <div className="owner-reservation-dates">
                  <div>
                    <CalendarDays size={18} />

                    <span>
                      <small>Preuzimanje</small>

                      {formatDate(
                        reservation.startDateTimeUtc
                      )}
                    </span>
                  </div>

                  <div>
                    <CalendarDays size={18} />

                    <span>
                      <small>Vraćanje</small>

                      {formatDate(
                        reservation.endDateTimeUtc
                      )}
                    </span>
                  </div>
                </div>

                <div className="travel-purpose">
                  <User size={17} />

                  <span>
                    <small>Svrha putovanja</small>
                    {reservation.travelPurpose}
                  </span>
                </div>

                {reservation.status === 0 && (
                  <div className="owner-actions">
                    <button
                      type="button"
                      className="reject-button"
                      onClick={() =>
                        changeStatus(
                          reservation.id,
                          "reject"
                        )
                      }
                    >
                      <X size={18} />
                      Odbij
                    </button>

                    <button
                      type="button"
                      className="approve-button"
                      onClick={() =>
                        changeStatus(
                          reservation.id,
                          "approve"
                        )
                      }
                    >
                      <Check size={18} />
                      Odobri
                    </button>
                  </div>
                )}

                {(reservation.status === 1 ||
                  reservation.status === 3 ||
                  reservation.status === 4) && (
                  <HandoverForm
                    reservation={reservation}
                    onUpdated={loadReservations}
                  />
                )}

                {reservation.status === 4 && (
                  <DisputeForm
                    reservationId={reservation.id}
                    onCreated={loadReservations}
                  />
                )}

                {reservation.status === 6 && (
                  <div className="dispute-message success">
                    Spor je otvoren i čeka odluku
                    administratora.
                  </div>
                )}
              </article>
            );
          })}
        </div>
      </main>
    </div>
  );
}

export default OwnerReservationsPage;