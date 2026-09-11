import { useState } from "react";
import {
  CheckCircle,
  Gauge,
  KeyRound,
  RotateCcw,
} from "lucide-react";

import api from "../api/api";
import FileUploadField from "./FileUploadField";
import ReviewForm from "./ReviewForm";
import "./HandoverForm.css";

function HandoverForm({
  reservation,
  onUpdated,
}) {
  const isPickup = reservation.status === 1;
  const isReturn = reservation.status === 3;
  const isCompleted = reservation.status === 4;

  const [isOpen, setIsOpen] = useState(false);
  const [mileage, setMileage] = useState("");
  const [photoUrl, setPhotoUrl] = useState("");
  const [message, setMessage] = useState("");
  const [isError, setIsError] = useState(false);
  const [isLoading, setIsLoading] =
    useState(false);
  const [confirmed, setConfirmed] =
    useState(false);

  if (isCompleted) {
    return (
      <ReviewForm reservationId={reservation.id} />
    );
  }

  if (!isPickup && !isReturn) {
    return null;
  }

  const mode = isPickup ? "pickup" : "return";

  const title = isPickup
    ? "Potvrdi preuzimanje"
    : "Potvrdi vraćanje";

  const submitHandover = async (event) => {
    event.preventDefault();

    const parsedMileage = Number(mileage);

    if (
      mileage === "" ||
      !Number.isFinite(parsedMileage) ||
      parsedMileage < 0
    ) {
      setIsError(true);
      setMessage(
        "Unesite ispravnu nenegativnu kilometražu."
      );
      return;
    }

    if (!photoUrl) {
      setIsError(true);
      setMessage(
        "Prvo izaberite i pošaljite fotografiju vozila."
      );
      return;
    }

    try {
      setIsLoading(true);
      setMessage("");
      setIsError(false);

      const requestBody = isPickup
        ? {
            pickupMileage: parsedMileage,
            pickupPhotoUrl: photoUrl,
          }
        : {
            returnMileage: parsedMileage,
            returnPhotoUrl: photoUrl,
          };

      await api.patch(
        `/reservations/${reservation.id}/handover/${mode}`,
        requestBody
      );

      setConfirmed(true);
      setIsOpen(false);

      setMessage(
        isPickup
          ? "Preuzimanje je potvrđeno sa vaše strane."
          : "Vraćanje je potvrđeno sa vaše strane."
      );

      if (onUpdated) {
        await onUpdated();
      }
    } catch (requestError) {
      setIsError(true);

      setMessage(
        requestError.response?.data?.detail ||
          "Potvrdu nije moguće evidentirati."
      );
    } finally {
      setIsLoading(false);
    }
  };

  if (confirmed) {
    return (
      <div className="handover-confirmed">
        <CheckCircle size={18} />
        {message}
      </div>
    );
  }

  return (
    <div className="handover-section">
      {!isOpen ? (
        <button
          type="button"
          className="open-handover-button"
          onClick={() => {
            setIsOpen(true);
            setMessage("");
            setIsError(false);
          }}
        >
          {isPickup ? (
            <KeyRound size={18} />
          ) : (
            <RotateCcw size={18} />
          )}

          {title}
        </button>
      ) : (
        <form
          className="handover-form"
          onSubmit={submitHandover}
        >
          <h3>{title}</h3>

          {isPickup && (
            <p>
              Preuzimanje je moguće nakon odobrene
              rezervacije i evidentiranog plaćanja.
            </p>
          )}

          {isReturn && (
            <p>
              Unesite stanje vozila prilikom vraćanja.
            </p>
          )}

          <label
            htmlFor={`handover-mileage-${reservation.id}`}
          >
            <span>
              <Gauge size={16} />
              Trenutna kilometraža
            </span>

            <input
              id={`handover-mileage-${reservation.id}`}
              type="number"
              min="0"
              step="1"
              value={mileage}
              onChange={(event) =>
                setMileage(event.target.value)
              }
              placeholder="Na primer: 62500"
              required
            />
          </label>

          <FileUploadField
            label={
              isPickup
                ? "Fotografija prilikom preuzimanja"
                : "Fotografija prilikom vraćanja"
            }
            accept=".jpg,.jpeg,.png,.webp"
            onUploaded={setPhotoUrl}
          />

          <div className="handover-buttons">
            <button
              type="button"
              className="handover-cancel-button"
              onClick={() => {
                setIsOpen(false);
                setMessage("");
                setIsError(false);
              }}
            >
              Odustani
            </button>

            <button
              type="submit"
              className="handover-confirm-button"
              disabled={
                isLoading ||
                !photoUrl ||
                mileage === ""
              }
            >
              {isLoading
                ? "Čuvanje..."
                : "Potvrdi"}
            </button>
          </div>
        </form>
      )}

      {message && (
        <div
          className={
            isError
              ? "handover-message error"
              : "handover-message success"
          }
        >
          {message}
        </div>
      )}
    </div>
  );
}

export default HandoverForm;