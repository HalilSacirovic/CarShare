import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  ArrowLeft,
  CalendarDays,
  MapPin,
  XCircle,
} from "lucide-react";

import api from "../api/api";
import PaymentForm from "../components/PaymentForm";
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

function MyReservationsPage() {
  const navigate = useNavigate();

  const [reservations, setReservations] = useState([]);
  const [paidReservationIds, setPaidReservationIds] =
    useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");

  const loadReservations = async () => {
    try {
      setError("");

      const response = await api.get(
        "/reservations/mine"
      );

      setReservations(response.data);
    } catch (requestError) {
      setError(
        requestError.response?.data?.detail ||
          "Nije moguće učitati rezervacije."
      );
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadReservations();
  }, []);

  const cancelReservation = async (reservationId) => {
    const confirmed = window.confirm(
      "Da li sigurno želite da otkažete rezervaciju?"
    );

    if (!confirmed) {
      return;
    }

    try {
      await api.patch(
        `/reservations/${reservationId}/cancel`
      );

      await loadReservations();
    } catch (requestError) {
      window.alert(
        requestError.response?.data?.detail ||
          "Rezervaciju nije moguće otkazati."
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
            VAŠA PUTOVANJA
          </p>

          <h1>Moje rezervacije</h1>

          <p>
            Pratite zahteve, plaćanje, primopredaju i
            završena putovanja.
          </p>
        </div>

        {isLoading && (
          <div className="status-card">
            Učitavanje rezervacija...
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
              Još uvek nemate rezervacije.
            </div>
          )}

        <div className="reservations-list">
          {reservations.map((reservation) => {
            const status =
              statusData[reservation.status] ??
              statusData[0];

            const paymentExists =
              reservation.isPaid === true ||
              Boolean(reservation.paymentId) ||
              reservation.paymentStatus === 1 ||
              paidReservationIds.includes(
                reservation.id
              );

            return (
              <article
                className="reservation-item"
                key={reservation.id}
              >
                <div className="reservation-car-mark">
                  {reservation.vehicleBrand
                    ?.charAt(0)
                    .toUpperCase()}
                </div>

                <div className="reservation-content">
                  <div className="reservation-top">
                    <div>
                      <h2>
                        {reservation.vehicleBrand}{" "}
                        {reservation.vehicleModel}
                      </h2>

                      <span
                        className={`reservation-status ${status.className}`}
                      >
                        {status.text}
                      </span>
                    </div>

                    <strong className="reservation-total">
                      {reservation.rentalAmount} €
                    </strong>
                  </div>

                  <div className="reservation-dates">
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

                  <div className="reservation-bottom">
                    <span>
                      <MapPin size={16} />
                      {reservation.travelPurpose}
                    </span>

                    {(reservation.status === 0 ||
                      reservation.status === 1) && (
                      <button
                        type="button"
                        onClick={() =>
                          cancelReservation(
                            reservation.id
                          )
                        }
                      >
                        <XCircle size={17} />
                        Otkaži
                      </button>
                    )}
                  </div>

                  {reservation.status === 1 &&
                    !paymentExists && (
                      <PaymentForm
                        reservation={reservation}
                        onPaid={(reservationId) =>
                          setPaidReservationIds(
                            (currentIds) =>
                              currentIds.includes(
                                reservationId
                              )
                                ? currentIds
                                : [
                                    ...currentIds,
                                    reservationId,
                                  ]
                          )
                        }
                      />
                    )}

                  {reservation.status === 1 &&
                    paymentExists && (
                      <div className="payment-success">
                        Plaćanje je evidentirano.
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
                      Spor je prijavljen i čeka odluku
                      administratora.
                    </div>
                  )}
                </div>
              </article>
            );
          })}
        </div>
      </main>
    </div>
  );
}

export default MyReservationsPage;