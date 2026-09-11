import { useState } from "react";
import { CreditCard, Send } from "lucide-react";

import api from "../api/api";
import FileUploadField from "./FileUploadField";
import "./DriverLicenseForm.css";

function DriverLicenseForm({ onSubmitted }) {
  const [driverLicenseUrl, setDriverLicenseUrl] =
    useState("");
  const [isLoading, setIsLoading] =
    useState(false);
  const [message, setMessage] = useState("");
  const [isError, setIsError] = useState(false);

  const submitDriverLicense = async (event) => {
    event.preventDefault();

    if (!driverLicenseUrl) {
      setIsError(true);
      setMessage(
        "Prvo izaberite i pošaljite dokument."
      );
      return;
    }

    try {
      setIsLoading(true);
      setMessage("");
      setIsError(false);

      await api.post("/users/me/driver-license", {
        driverLicenseUrl,
      });

      setMessage(
        "Vozačka dozvola je poslata administratoru na proveru."
      );

      if (onSubmitted) {
        await onSubmitted();
      }
    } catch (requestError) {
      setIsError(true);

      setMessage(
        requestError.response?.data?.detail ||
          "Vozačku dozvolu nije moguće poslati."
      );
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <form
      className="driver-license-form"
      onSubmit={submitDriverLicense}
    >
      <div className="driver-license-heading">
        <span>
          <CreditCard size={23} />
        </span>

        <div>
          <h2>Verifikacija vozača</h2>

          <p>
            Pošaljite fotografiju ili PDF svoje vozačke
            dozvole.
          </p>
        </div>
      </div>

      <FileUploadField
        label="Vozačka dozvola"
        accept=".jpg,.jpeg,.png,.webp,.pdf"
        onUploaded={setDriverLicenseUrl}
      />

      <button
        type="submit"
        disabled={isLoading || !driverLicenseUrl}
      >
        <Send size={17} />

        {isLoading
          ? "Slanje..."
          : "Pošalji na verifikaciju"}
      </button>

      {message && (
        <div
          className={
            isError
              ? "driver-license-message error"
              : "driver-license-message success"
          }
        >
          {message}
        </div>
      )}
    </form>
  );
}

export default DriverLicenseForm;