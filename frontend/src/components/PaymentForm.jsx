import { useState } from "react";
import {
  CheckCircle,
  CreditCard,
  ShieldCheck,
} from "lucide-react";

import api from "../api/api";
import "./PaymentForm.css";

function formatCardNumber(value) {
  return value
    .replace(/\D/g, "")
    .slice(0, 16)
    .replace(/(\d{4})(?=\d)/g, "$1 ");
}

export default function PaymentForm({
  reservation,
  onPaid,
}) {
  const [isOpen, setIsOpen] = useState(false);
  const [cardNumber, setCardNumber] = useState("");
  const [message, setMessage] = useState("");
  const [isError, setIsError] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [isPaid, setIsPaid] = useState(false);

  const submitPayment = async (event) => {
    event.preventDefault();

    const cleanCardNumber = cardNumber.replace(/\s/g, "");

    if (!/^\d{16}$/.test(cleanCardNumber)) {
      setIsError(true);
      setMessage(
        "Broj kartice mora sadržati tačno 16 cifara."
      );
      return;
    }

    try {
      setIsLoading(true);
      setMessage("");
      setIsError(false);

      await api.post(
        `/reservations/${reservation.id}/payment`,
        {
          paymentMethod: "MockCard",
          cardNumber: cleanCardNumber,
        }
      );

      setIsPaid(true);
      setIsOpen(false);
      setCardNumber("");
      setMessage("Plaćanje je uspešno evidentirano.");

      onPaid?.(reservation.id);
    } catch (error) {
      setIsError(true);

      setMessage(
        error.response?.data?.detail ||
          "Plaćanje nije moguće izvršiti."
      );
    } finally {
      setIsLoading(false);
    }
  };

  if (isPaid) {
    return (
      <div className="payment-success">
        <CheckCircle size={18} />
        Plaćeno
      </div>
    );
  }

  return (
    <div className="payment-section">
      {!isOpen ? (
        <button
          type="button"
          className="open-payment-button"
          onClick={() => {
            setIsOpen(true);
            setMessage("");
          }}
        >
          <CreditCard size={18} />
          Plati rezervaciju
        </button>
      ) : (
        <form
          className="payment-form"
          onSubmit={submitPayment}
        >
          <div className="payment-form-heading">
            <CreditCard size={20} />

            <div>
              <strong>Mock plaćanje</strong>
              <small>
                Kartica neće biti stvarno naplaćena.
              </small>
            </div>
          </div>

          <label htmlFor={`card-${reservation.id}`}>
            Broj kartice
          </label>

          <input
            id={`card-${reservation.id}`}
            type="text"
            inputMode="numeric"
            autoComplete="off"
            value={cardNumber}
            onChange={(event) => {
              setCardNumber(
                formatCardNumber(event.target.value)
              );

              setMessage("");
            }}
            placeholder="4111 1111 1111 1111"
            required
          />

          <div className="payment-summary">
            <span>
              Cena najma
              <strong>
                {Number(
                  reservation.rentalAmount ?? 0
                ).toFixed(2)}{" "}
                €
              </strong>
            </span>

            <span>
              Depozit
              <strong>
                {Number(
                  reservation.depositAmount ?? 0
                ).toFixed(2)}{" "}
                €
              </strong>
            </span>
          </div>

          <div className="payment-security">
            <ShieldCheck size={15} />
            Testno plaćanje za potrebe projekta
          </div>

          <div className="payment-buttons">
            <button
              type="button"
              className="cancel-payment-button"
              onClick={() => {
                setIsOpen(false);
                setCardNumber("");
                setMessage("");
              }}
            >
              Odustani
            </button>

            <button
              type="submit"
              className="confirm-payment-button"
              disabled={isLoading}
            >
              {isLoading ? "Plaćanje..." : "Potvrdi"}
            </button>
          </div>
        </form>
      )}

      {message && (
        <div
          className={
            isError
              ? "payment-message error"
              : "payment-message success"
          }
        >
          {message}
        </div>
      )}
    </div>
  );
}