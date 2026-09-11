import { useState } from "react";
import { AlertTriangle } from "lucide-react";

import api from "../api/api";
import "./DisputeForm.css";

function DisputeForm({ reservationId, onCreated }) {
  const [isOpen, setIsOpen] = useState(false);
  const [reason, setReason] = useState("");
  const [description, setDescription] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [message, setMessage] = useState("");
  const [isError, setIsError] = useState(false);

  const submitDispute = async (event) => {
    event.preventDefault();

    try {
      setIsLoading(true);
      setMessage("");
      setIsError(false);

      await api.post(
        `/reservations/${reservationId}/disputes`,
        {
          reason,
          description,
        }
      );

      setMessage("Spor je uspešno prijavljen.");
      setReason("");
      setDescription("");

      if (onCreated) {
        await onCreated();
      }
    } catch (requestError) {
      setIsError(true);

      setMessage(
        requestError.response?.data?.detail ||
          "Spor nije moguće prijaviti."
      );
    } finally {
      setIsLoading(false);
    }
  };

  if (!isOpen) {
    return (
      <button
        type="button"
        className="open-dispute-button"
        onClick={() => setIsOpen(true)}
      >
        <AlertTriangle size={17} />
        Prijavi problem
      </button>
    );
  }

  return (
    <div className="dispute-form-container">
      <div className="dispute-form-heading">
        <div>
          <AlertTriangle size={20} />

          <div>
            <strong>Prijava problema</strong>
            <span>
              Opišite problem koji se dogodio tokom najma.
            </span>
          </div>
        </div>

        <button
          type="button"
          className="close-dispute-button"
          onClick={() => {
            setIsOpen(false);
            setMessage("");
          }}
        >
          Zatvori
        </button>
      </div>

      <form
        className="dispute-form"
        onSubmit={submitDispute}
      >
        <label htmlFor={`dispute-reason-${reservationId}`}>
          Razlog
        </label>

        <select
          id={`dispute-reason-${reservationId}`}
          value={reason}
          onChange={(event) =>
            setReason(event.target.value)
          }
          required
        >
          <option value="">Izaberite razlog</option>
          <option value="Oštećenje vozila">
            Oštećenje vozila
          </option>
          <option value="Problem sa primopredajom">
            Problem sa primopredajom
          </option>
          <option value="Problem sa plaćanjem">
            Problem sa plaćanjem
          </option>
          <option value="Vozilo nije bilo kao u oglasu">
            Vozilo nije bilo kao u oglasu
          </option>
          <option value="Neprimereno ponašanje">
            Neprimereno ponašanje
          </option>
          <option value="Drugo">Drugo</option>
        </select>

        <label
          htmlFor={`dispute-description-${reservationId}`}
        >
          Opis problema
        </label>

        <textarea
          id={`dispute-description-${reservationId}`}
          value={description}
          onChange={(event) =>
            setDescription(event.target.value)
          }
          placeholder="Detaljno opišite šta se dogodilo..."
          minLength={10}
          maxLength={1000}
          required
        />

        <div className="dispute-form-actions">
          <button
            type="button"
            className="cancel-dispute-button"
            onClick={() => {
              setIsOpen(false);
              setMessage("");
            }}
          >
            Odustani
          </button>

          <button
            type="submit"
            className="submit-dispute-button"
            disabled={isLoading}
          >
            {isLoading
              ? "Slanje..."
              : "Pošalji prijavu"}
          </button>
        </div>
      </form>

      {message && (
        <div
          className={
            isError
              ? "dispute-message error"
              : "dispute-message success"
          }
        >
          {message}
        </div>
      )}
    </div>
  );
}

export default DisputeForm;