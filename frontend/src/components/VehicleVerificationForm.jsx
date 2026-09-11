import { useState } from "react";
import { Send, ShieldCheck } from "lucide-react";

import api from "../api/api";
import FileUploadField from "./FileUploadField";
import "./VehicleVerificationForm.css";

function VehicleVerificationForm({
  vehicleId,
  onSubmitted,
}) {
  const [documentUrl, setDocumentUrl] =
    useState("");
  const [isLoading, setIsLoading] =
    useState(false);
  const [message, setMessage] = useState("");
  const [isError, setIsError] = useState(false);

  const submitVerification = async (event) => {
    event.preventDefault();

    if (!documentUrl) {
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

      await api.patch(
        `/vehicles/${vehicleId}/verification`,
        {
          registrationDocumentUrl: documentUrl,
        }
      );

      setMessage(
        "Dokument je poslat administratoru na proveru."
      );

      if (onSubmitted) {
        await onSubmitted();
      }
    } catch (requestError) {
      setIsError(true);

      setMessage(
        requestError.response?.data?.detail ||
          "Dokument nije moguće poslati na proveru."
      );
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <form
      className="vehicle-verification-form"
      onSubmit={submitVerification}
    >
      <div className="vehicle-verification-heading">
        <ShieldCheck size={19} />

        <div>
          <strong>Verifikacija vozila</strong>
          <span>
            Pošaljite saobraćajnu dozvolu ili drugi
            dokument vozila.
          </span>
        </div>
      </div>

      <FileUploadField
        label="Dokument vozila"
        accept=".jpg,.jpeg,.png,.webp,.pdf"
        onUploaded={setDocumentUrl}
      />

      <button
        type="submit"
        disabled={isLoading || !documentUrl}
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
              ? "vehicle-verification-message error"
              : "vehicle-verification-message success"
          }
        >
          {message}
        </div>
      )}
    </form>
  );
}

export default VehicleVerificationForm;