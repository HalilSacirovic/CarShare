import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  AlertTriangle,
  ArrowLeft,
  CalendarDays,
  CheckCircle,
  Mail,
  ShieldAlert,
  User,
  XCircle,
} from "lucide-react";

import api from "../api/api";
import "./AdminDisputesPage.css";

function formatDate(date) {
  if (!date) {
    return "Datum nije dostupan";
  }

  return new Intl.DateTimeFormat("sr-RS", {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(new Date(date));
}

function AdminDisputesPage() {
  const navigate = useNavigate();

  const [disputes, setDisputes] = useState([]);
  const [decisions, setDecisions] = useState({});
  const [isLoading, setIsLoading] = useState(true);
  const [activeDisputeId, setActiveDisputeId] =
    useState(null);
  const [error, setError] = useState("");

  const loadDisputes = async () => {
    try {
      setError("");

      const response = await api.get(
        "/admin/disputes/open"
      );

      setDisputes(response.data);
    } catch (requestError) {
      setError(
        requestError.response?.status === 403
          ? "Ova stranica je dostupna samo administratoru."
          : requestError.response?.data?.detail ||
              "Nije moguće učitati otvorene sporove."
      );
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadDisputes();
  }, []);

  const handleDecisionChange = (
    disputeId,
    value
  ) => {
    setDecisions((currentDecisions) => ({
      ...currentDecisions,
      [disputeId]: value,
    }));
  };

  const processDispute = async (
    disputeId,
    action
  ) => {
    const adminDecision =
      decisions[disputeId]?.trim();

    if (!adminDecision) {
      window.alert(
        "Prvo unesite obrazloženje odluke administratora."
      );
      return;
    }

    const actionText =
      action === "resolve" ? "rešite" : "odbijete";

    const confirmed = window.confirm(
      `Da li sigurno želite da ${actionText} ovaj spor?`
    );

    if (!confirmed) {
      return;
    }

    try {
      setActiveDisputeId(disputeId);

      await api.patch(
        `/admin/disputes/${disputeId}/${action}`,
        {
          adminDecision,
        }
      );

      setDisputes((currentDisputes) =>
        currentDisputes.filter(
          (dispute) =>
            dispute.disputeId !== disputeId
        )
      );

      setDecisions((currentDecisions) => {
        const updatedDecisions = {
          ...currentDecisions,
        };

        delete updatedDecisions[disputeId];

        return updatedDecisions;
      });
    } catch (requestError) {
      window.alert(
        requestError.response?.data?.detail ||
          "Odluku nije moguće sačuvati."
      );
    } finally {
      setActiveDisputeId(null);
    }
  };

  return (
    <div className="admin-disputes-page">
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

      <main className="admin-disputes-container">
        <div className="admin-disputes-heading">
          <div className="admin-disputes-icon">
            <ShieldAlert size={28} />
          </div>

          <div>
            <p className="section-label">
              ADMINISTRACIJA
            </p>

            <h1>Otvoreni sporovi</h1>

            <p>
              Pregledajte prijavljene probleme i donesite
              odluku na osnovu podataka o rezervaciji.
            </p>
          </div>
        </div>

        {isLoading && (
          <div className="status-card">
            Učitavanje sporova...
          </div>
        )}

        {error && (
          <div className="status-card error-message">
            {error}
          </div>
        )}

        {!isLoading &&
          !error &&
          disputes.length === 0 && (
            <div className="admin-disputes-empty">
              <CheckCircle size={44} />

              <h2>Nema otvorenih sporova</h2>

              <p>
                Svi prijavljeni sporovi su trenutno
                obrađeni.
              </p>
            </div>
          )}

        <div className="admin-disputes-list">
          {disputes.map((dispute) => {
            const isProcessing =
              activeDisputeId === dispute.disputeId;

            return (
              <article
                className="admin-dispute-card"
                key={dispute.disputeId}
              >
                <div className="admin-dispute-top">
                  <div>
                    <span className="admin-dispute-badge">
                      <AlertTriangle size={15} />
                      Otvoren spor
                    </span>

                    <h2>
                      {dispute.vehicleBrand}{" "}
                      {dispute.vehicleModel}
                    </h2>
                  </div>

                  <span className="admin-dispute-date">
                    <CalendarDays size={16} />
                    {formatDate(dispute.openedAtUtc)}
                  </span>
                </div>

                <div className="admin-dispute-user">
                  <div className="admin-dispute-avatar">
                    {dispute.openedByName
                      ?.charAt(0)
                      .toUpperCase()}
                  </div>

                  <div>
                    <span>
                      <User size={15} />
                      Prijavio
                    </span>

                    <strong>
                      {dispute.openedByName}
                    </strong>

                    <small>
                      <Mail size={14} />
                      {dispute.openedByEmail}
                    </small>
                  </div>
                </div>

                <div className="admin-dispute-details">
                  <div>
                    <span>Razlog</span>
                    <strong>{dispute.reason}</strong>
                  </div>

                  <div>
                    <span>Opis problema</span>
                    <p>{dispute.description}</p>
                  </div>
                </div>

                <label
                  className="admin-decision-field"
                  htmlFor={`decision-${dispute.disputeId}`}
                >
                  Odluka administratora

                  <textarea
                    id={`decision-${dispute.disputeId}`}
                    value={
                      decisions[dispute.disputeId] ?? ""
                    }
                    onChange={(event) =>
                      handleDecisionChange(
                        dispute.disputeId,
                        event.target.value
                      )
                    }
                    placeholder="Napišite obrazloženje odluke..."
                    minLength={5}
                    maxLength={1000}
                    disabled={isProcessing}
                  />
                </label>

                <div className="admin-dispute-actions">
                  <button
                    type="button"
                    className="admin-reject-dispute"
                    disabled={isProcessing}
                    onClick={() =>
                      processDispute(
                        dispute.disputeId,
                        "reject"
                      )
                    }
                  >
                    <XCircle size={18} />
                    Odbij spor
                  </button>

                  <button
                    type="button"
                    className="admin-resolve-dispute"
                    disabled={isProcessing}
                    onClick={() =>
                      processDispute(
                        dispute.disputeId,
                        "resolve"
                      )
                    }
                  >
                    <CheckCircle size={18} />

                    {isProcessing
                      ? "Čuvanje..."
                      : "Reši spor"}
                  </button>
                </div>
              </article>
            );
          })}
        </div>
      </main>
    </div>
  );
}

export default AdminDisputesPage;