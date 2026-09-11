import { useEffect, useState } from "react";
import { ArrowLeft, CalendarDays, Trash2 } from "lucide-react";
import { useNavigate, useParams } from "react-router-dom";


import api from "../api/api";

export default function VehicleAvailabilityPage() {
  const { vehicleId } = useParams();
  const navigate = useNavigate();

  const [blocks, setBlocks] = useState([]);
  const [startDateTimeUtc, setStartDateTimeUtc] = useState("");
  const [endDateTimeUtc, setEndDateTimeUtc] = useState("");
  const [reason, setReason] = useState("");
  const [message, setMessage] = useState("");
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  const loadBlocks = async () => {
    try {
      setMessage("");

      const response = await api.get(
        `/vehicles/${vehicleId}/availability-blocks`
      );

      setBlocks(response.data);
    } catch (error) {
      setMessage(
        error.response?.data?.detail ||
          "Nije moguće učitati blokirane termine."
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadBlocks();
  }, [vehicleId]);

  const handleSubmit = async (event) => {
    event.preventDefault();

    const start = new Date(startDateTimeUtc);
    const end = new Date(endDateTimeUtc);

    if (start >= end) {
      setMessage(
        "Datum završetka mora biti posle datuma početka."
      );
      return;
    }

    try {
      setSaving(true);
      setMessage("");

      await api.post(`/vehicles/${vehicleId}/availability`, {
        startDateTimeUtc: start.toISOString(),
        endDateTimeUtc: end.toISOString(),
        reason,
      });

      setStartDateTimeUtc("");
      setEndDateTimeUtc("");
      setReason("");
      setMessage("Termin je uspešno blokiran.");

      await loadBlocks();
    } catch (error) {
      setMessage(
        error.response?.data?.detail ||
          "Termin nije moguće blokirati."
      );
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (availabilityBlockId) => {
    const confirmed = window.confirm(
      "Da li želite da uklonite ovaj blokirani termin?"
    );

    if (!confirmed) {
      return;
    }

    try {
      setMessage("");

      await api.delete(
        `/vehicles/availability-blocks/${availabilityBlockId}`
      );

      setBlocks((currentBlocks) =>
        currentBlocks.filter(
          (block) => block.id !== availabilityBlockId
        )
      );

      setMessage("Blokirani termin je uklonjen.");
    } catch (error) {
      setMessage(
        error.response?.data?.detail ||
          "Termin nije moguće ukloniti."
      );
    }
  };

  return (
    <div className="availability-page">
      <header className="details-header">
        <button
          className="brand"
          onClick={() => navigate("/vehicles")}
        >
          <span className="brand-mark">◆</span>
          <span>CarShare</span>
        </button>

        <button
          className="back-button"
          onClick={() => navigate("/owner/vehicles")}
        >
          <ArrowLeft size={18} />
          Moja vozila
        </button>
      </header>

      <main className="availability-container">
        <div className="vehicle-form-heading">
          <span className="vehicle-form-icon">
            <CalendarDays size={28} />
          </span>

          <div>
            <p className="section-label">DOSTUPNOST</p>
            <h1>Blokirani termini</h1>
            <p>
              Dodajte periode tokom kojih vozilo nije dostupno.
            </p>
          </div>
        </div>

        <form
          className="availability-form"
          onSubmit={handleSubmit}
        >
          <label>
            Početak perioda
            <input
              type="datetime-local"
              value={startDateTimeUtc}
              onChange={(event) =>
                setStartDateTimeUtc(event.target.value)
              }
              required
            />
          </label>

          <label>
            Kraj perioda
            <input
              type="datetime-local"
              min={startDateTimeUtc}
              value={endDateTimeUtc}
              onChange={(event) =>
                setEndDateTimeUtc(event.target.value)
              }
              required
            />
          </label>

          <label>
            Razlog
            <input
              value={reason}
              onChange={(event) =>
                setReason(event.target.value)
              }
              placeholder="Servis vozila"
              required
            />
          </label>

          <button type="submit" disabled={saving}>
            {saving ? "Čuvanje..." : "Blokiraj termin"}
          </button>
        </form>

        {message && (
          <div className="reservation-message">
            {message}
          </div>
        )}

        <section className="availability-list">
          <h2>Trenutno blokirani periodi</h2>

          {loading && <p>Učitavanje...</p>}

          {!loading && blocks.length === 0 && (
            <p>Nema blokiranih termina.</p>
          )}

          {blocks.map((block) => (
            <article
              className="availability-card"
              key={block.id}
            >
              <div>
                <strong>{block.reason}</strong>

                <p>
                  {new Date(
                    block.startDateTimeUtc
                  ).toLocaleString("sr-RS")}
                  {" — "}
                  {new Date(
                    block.endDateTimeUtc
                  ).toLocaleString("sr-RS")}
                </p>
              </div>

              <button
                type="button"
                onClick={() => handleDelete(block.id)}
                aria-label="Obriši blokirani termin"
              >
                <Trash2 size={18} />
              </button>
            </article>
          ))}
        </section>
      </main>
    </div>
  );
}