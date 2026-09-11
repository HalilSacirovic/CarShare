import { useEffect, useState } from "react";
import { Check, ExternalLink, FileText, X } from "lucide-react";
import api from "../api/api";

export default function AdminDriverVerificationsPage() {
  const [requests, setRequests] = useState([]);
  const [loading, setLoading] = useState(true);
  const [processingId, setProcessingId] = useState(null);
  const [error, setError] = useState("");

  const loadRequests = async () => {
    try {
      setError("");

      const response = await api.get(
        "/admin/driver-verifications/pending"
      );

      setRequests(response.data);
    } catch (requestError) {
      setError(
        requestError.response?.data?.detail ||
          "Nije moguće učitati zahteve."
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadRequests();
  }, []);

  const handleDecision = async (userId, decision) => {
    try {
      setProcessingId(userId);
      setError("");

      await api.patch(
        `/admin/driver-verifications/${userId}/${decision}`
      );

      setRequests((currentRequests) =>
        currentRequests.filter(
          (request) => request.userId !== userId
        )
      );
    } catch (requestError) {
      setError(
        requestError.response?.data?.detail ||
          "Nije moguće obraditi zahtev."
      );
    } finally {
      setProcessingId(null);
    }
  };

  if (loading) {
    return (
      <main className="admin-verifications-page">
        <p>Učitavanje zahteva...</p>
      </main>
    );
  }

  return (
    <main className="admin-verifications-page">
      <section className="admin-page-heading">
        <span className="eyebrow">ADMINISTRACIJA</span>
        <h1>Provera vozačkih dozvola</h1>
        <p>
          Pregledajte dokumente korisnika i odobrite ili odbijte
          njihove zahteve.
        </p>
      </section>

      {error && <div className="form-error">{error}</div>}

      {requests.length === 0 ? (
        <section className="empty-verifications">
          <FileText size={38} />
          <h2>Nema zahteva na čekanju</h2>
          <p>Sve prijavljene vozačke dozvole su obrađene.</p>
        </section>
      ) : (
        <section className="verification-list">
          {requests.map((request) => (
            <article
              className="verification-request-card"
              key={request.userId}
            >
              <div className="verification-request-info">
                <span className="pending-label">
                  Čeka proveru
                </span>

                <h2>
                  {request.firstName} {request.lastName}
                </h2>

                <p>{request.email}</p>

                <p className="submitted-date">
                  Poslato:{" "}
                  {request.submittedAtUtc
                    ? new Date(
                        request.submittedAtUtc
                      ).toLocaleString("sr-RS")
                    : "Datum nije dostupan"}
                </p>
              </div>

              <div className="verification-actions">
                <a
                  className="document-link"
                  href={request.driverLicenseUrl}
                  target="_blank"
                  rel="noreferrer"
                >
                  <ExternalLink size={17} />
                  Otvori dokument
                </a>

                <button
                  className="approve-button"
                  disabled={processingId === request.userId}
                  onClick={() =>
                    handleDecision(request.userId, "approve")
                  }
                >
                  <Check size={18} />
                  Odobri
                </button>

                <button
                  className="reject-button"
                  disabled={processingId === request.userId}
                  onClick={() =>
                    handleDecision(request.userId, "reject")
                  }
                >
                  <X size={18} />
                  Odbij
                </button>
              </div>
            </article>
          ))}
        </section>
      )}
    </main>
  );
}