import {
  useEffect,
  useMemo,
  useState,
} from "react";
import { useNavigate } from "react-router-dom";
import {
  CalendarCheck,
  Clock,
  ShieldAlert,
} from "lucide-react";

import api from "../api/api";
import "./ReservationForm.css";

function formatForDateTimeInput(date) {
  const timezoneOffset =
    date.getTimezoneOffset() * 60000;

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

function getErrorMessage(error) {
  const validationErrors =
    error.response?.data?.errors;

  if (validationErrors) {
    return (
      Object.values(validationErrors)[0]?.[0] ??
      "Proverite podatke rezervacije."
    );
  }

  return (
    error.response?.data?.detail ??
    "Rezervaciju nije moguće napraviti."
  );
}

function ReservationForm({ vehicle }) {
  const navigate = useNavigate();

  const [profile, setProfile] = useState(null);
  const [isProfileLoading, setIsProfileLoading] =
    useState(true);
  const [profileError, setProfileError] = useState("");

  const [startDateTime, setStartDateTime] = useState(
    createFutureDate(1)
  );

  const [endDateTime, setEndDateTime] = useState(
    createFutureDate(2)
  );

  const [travelPurpose, setTravelPurpose] =
    useState("");

  const [
    unavailablePeriods,
    setUnavailablePeriods,
  ] = useState([]);

  const [message, setMessage] = useState("");
  const [isError, setIsError] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const minimumDateTime =
    formatForDateTimeInput(new Date());

  useEffect(() => {
    const loadProfile = async () => {
      try {
        setProfileError("");

        const response = await api.get("/users/me");

        setProfile(response.data);
      } catch (requestError) {
        setProfileError(
          requestError.response?.data?.detail ??
            "Nije moguće proveriti korisnički profil."
        );
      } finally {
        setIsProfileLoading(false);
      }
    };

    loadProfile();
  }, []);

  useEffect(() => {
    const loadUnavailablePeriods = async () => {
      try {
        const response = await api.get(
          `/vehicles/${vehicle.id}/calendar`
        );

        setUnavailablePeriods(response.data);
      } catch {
        // Backend ponovo proverava dostupnost
        // prilikom kreiranja rezervacije.
        setUnavailablePeriods([]);
      }
    };

    loadUnavailablePeriods();
  }, [vehicle.id]);

  const reservationSummary = useMemo(() => {
    if (!startDateTime || !endDateTime) {
      return null;
    }

    const start = new Date(startDateTime);
    const end = new Date(endDateTime);

    if (
      Number.isNaN(start.getTime()) ||
      Number.isNaN(end.getTime()) ||
      end <= start
    ) {
      return null;
    }

    const milliseconds =
      end.getTime() - start.getTime();

    const numberOfDays = Math.max(
      1,
      Math.ceil(
        milliseconds /
          (1000 * 60 * 60 * 24)
      )
    );

    const totalPrice =
      numberOfDays *
      Number(vehicle.pricePerDay ?? 0);

    return {
      numberOfDays,
      totalPrice,
    };
  }, [
    startDateTime,
    endDateTime,
    vehicle.pricePerDay,
  ]);

  const periodOverlaps = (start, end) =>
    unavailablePeriods.some((period) => {
      const unavailableStart = new Date(
        period.startDateTimeUtc
      );

      const unavailableEnd = new Date(
        period.endDateTimeUtc
      );

      return (
        start < unavailableEnd &&
        end > unavailableStart
      );
    });

  const submitReservation = async (event) => {
    event.preventDefault();

    const start = new Date(startDateTime);
    const end = new Date(endDateTime);
    const currentDate = new Date();

    setMessage("");
    setIsError(false);

    if (start <= currentDate) {
      setIsError(true);
      setMessage(
        "Datum preuzimanja mora biti u budućnosti."
      );
      return;
    }

    if (end <= start) {
      setIsError(true);
      setMessage(
        "Datum vraćanja mora biti posle datuma preuzimanja."
      );
      return;
    }

    if (periodOverlaps(start, end)) {
      setIsError(true);
      setMessage(
        "Vozilo nije dostupno tokom izabranog perioda. Pogledajte crvene datume u kalendaru."
      );
      return;
    }

    try {
      setIsLoading(true);

      const response = await api.post(
        "/reservations",
        {
          vehicleId: vehicle.id,
          startDateTimeUtc:
            start.toISOString(),
          endDateTimeUtc: end.toISOString(),
          travelPurpose:
            travelPurpose.trim(),
        }
      );

      const reservationId =
        response.data?.id ?? response.data;

      setIsError(false);
      setMessage(
        `Rezervacija je uspešno poslata. ID: ${reservationId}`
      );

      setStartDateTime(createFutureDate(1));
      setEndDateTime(createFutureDate(2));
      setTravelPurpose("");
    } catch (error) {
      setIsError(true);
      setMessage(getErrorMessage(error));
    } finally {
      setIsLoading(false);
    }
  };

  if (isProfileLoading) {
    return (
      <aside className="reservation-card">
        <p>Rezervišite ovo vozilo</p>

        <div className="status-card">
          Provera korisničkog profila...
        </div>
      </aside>
    );
  }

  if (profileError) {
    return (
      <aside className="reservation-card">
        <p>Rezervišite ovo vozilo</p>

        <div className="reservation-message error">
          {profileError}
        </div>
      </aside>
    );
  }

  if (!profile?.isRenter) {
    return (
      <aside className="reservation-card">
        <p>Rezervacija vozila</p>

        <div className="reservation-form">
          <div className="reservation-message error">
            <ShieldAlert size={22} />
            Samo korisnik sa renter ulogom može
            rezervisati vozilo.
          </div>

          <button
            type="button"
            onClick={() => navigate("/vehicles")}
          >
            Nazad na vozila
          </button>
        </div>
      </aside>
    );
  }

  const isDriverLicenseApproved =
    profile.driverVerificationStatus === 2 ||
    profile.driverVerificationStatus ===
      "Approved";

  if (!isDriverLicenseApproved) {
    return (
      <aside className="reservation-card">
        <p>Rezervacija vozila</p>

        <div className="reservation-form">
          <div className="reservation-message error">
            <ShieldAlert size={22} />
            Morate imati odobrenu vozačku dozvolu pre
            rezervacije vozila.
          </div>

          <button
            type="button"
            onClick={() => navigate("/profile")}
          >
            Idi na profil
          </button>
        </div>

        <small className="reservation-payment-note">
          Pošaljite vozačku dozvolu i sačekajte
          odobrenje administratora.
        </small>
      </aside>
    );
  }

  return (
    <aside className="reservation-card">
      <p>Rezervišite ovo vozilo</p>

      <div className="reservation-price">
        <strong>{vehicle.pricePerDay} €</strong>
        <span>/ dan</span>
      </div>

      <form
        className="reservation-form"
        onSubmit={submitReservation}
      >
        <label htmlFor="startDateTime">
          Datum i vreme preuzimanja
        </label>

        <input
          id="startDateTime"
          type="datetime-local"
          min={minimumDateTime}
          value={startDateTime}
          onChange={(event) => {
            setStartDateTime(
              event.target.value
            );
            setMessage("");
          }}
          required
        />

        <label htmlFor="endDateTime">
          Datum i vreme vraćanja
        </label>

        <input
          id="endDateTime"
          type="datetime-local"
          min={
            startDateTime ||
            minimumDateTime
          }
          value={endDateTime}
          onChange={(event) => {
            setEndDateTime(
              event.target.value
            );
            setMessage("");
          }}
          required
        />

        <label htmlFor="travelPurpose">
          Svrha putovanja
        </label>

        <textarea
          id="travelPurpose"
          value={travelPurpose}
          onChange={(event) =>
            setTravelPurpose(
              event.target.value
            )
          }
          placeholder="Na primer: Poslovno putovanje"
          minLength={3}
          maxLength={500}
          required
        />

        {reservationSummary && (
          <div className="reservation-estimate">
            <span>
              <Clock size={17} />
              Trajanje
              <strong>
                {reservationSummary.numberOfDays}{" "}
                {reservationSummary.numberOfDays === 1
                  ? "dan"
                  : "dana"}
              </strong>
            </span>

            <span>
              <CalendarCheck size={17} />
              Procena najma
              <strong>
                {reservationSummary.totalPrice.toFixed(
                  2
                )}{" "}
                €
              </strong>
            </span>

            <small>
              Depozit od {vehicle.depositAmount} € nije
              uračunat u cenu najma.
            </small>
          </div>
        )}

        <button
          type="submit"
          disabled={isLoading}
        >
          {isLoading
            ? "Slanje zahteva..."
            : "Pošalji zahtev"}
        </button>
      </form>

      {message && (
        <div
          className={
            isError
              ? "reservation-message error"
              : "reservation-message success"
          }
        >
          {message}
        </div>
      )}

      <small className="reservation-payment-note">
        Plaćanje se vrši tek nakon potvrđene
        rezervacije.
      </small>
    </aside>
  );
}

export default ReservationForm;