import { useState } from "react";
import {
  CheckCircle,
  FileUp,
  LoaderCircle,
} from "lucide-react";

import api from "../api/api";
import "./FileUploadField.css";

function FileUploadField({
  label = "Izaberite fajl",
  accept = ".jpg,.jpeg,.png,.webp,.pdf",
  onUploaded,
  required = false,
}) {
  const [fileName, setFileName] = useState("");
  const [uploadedUrl, setUploadedUrl] =
    useState("");
  const [isUploading, setIsUploading] =
    useState(false);
  const [error, setError] = useState("");

  const uploadFile = async (event) => {
    const file = event.target.files?.[0];

    if (!file) {
      return;
    }

    if (file.size > 10 * 1024 * 1024) {
      setError(
        "Fajl je prevelik. Maksimalna veličina je 10 MB."
      );
      event.target.value = "";
      return;
    }

    try {
      setIsUploading(true);
      setError("");
      setUploadedUrl("");
      setFileName(file.name);

      const formData = new FormData();
      formData.append("file", file);

      const response = await api.post(
        "/uploads",
        formData,
        {
          headers: {
            "Content-Type": "multipart/form-data",
          },
        }
      );

      const url = response.data.url;

      setUploadedUrl(url);

      if (onUploaded) {
        onUploaded(url);
      }
    } catch (requestError) {
      setFileName("");
      setUploadedUrl("");

      setError(
        requestError.response?.data?.detail ||
          "Fajl nije moguće poslati."
      );

      event.target.value = "";
    } finally {
      setIsUploading(false);
    }
  };

  return (
    <div className="file-upload-field">
      <label className="file-upload-label">
        <span>{label}</span>

        <div
          className={`file-upload-box ${
            uploadedUrl ? "uploaded" : ""
          }`}
        >
          {isUploading ? (
            <LoaderCircle
              className="file-upload-spinner"
              size={23}
            />
          ) : uploadedUrl ? (
            <CheckCircle size={23} />
          ) : (
            <FileUp size={23} />
          )}

          <div>
            <strong>
              {isUploading
                ? "Slanje fajla..."
                : uploadedUrl
                  ? "Fajl je uspešno poslat"
                  : "Kliknite i izaberite fajl"}
            </strong>

            <small>
              {fileName ||
                "JPG, PNG, WEBP ili PDF — do 10 MB"}
            </small>
          </div>

          <input
            type="file"
            accept={accept}
            onChange={uploadFile}
            disabled={isUploading}
            required={required && !uploadedUrl}
          />
        </div>
      </label>

      {uploadedUrl && (
        <a
          className="uploaded-file-link"
          href={uploadedUrl}
          target="_blank"
          rel="noreferrer"
        >
          Otvori poslati fajl
        </a>
      )}

      {error && (
        <div className="file-upload-error">
          {error}
        </div>
      )}
    </div>
  );
}

export default FileUploadField;