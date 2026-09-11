import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  AlertTriangle,
  ArrowLeft,
  Car,
  CheckCircle,
  CircleDollarSign,
  RefreshCw,
  Users,
} from "lucide-react";

import api from "../api/api";
import "./AdminStatisticsPage.css";

function formatMoney(value) {
  return new Intl.NumberFormat("sr-RS", {
    style: "currency",
    currency: "EUR",
  }).format(Number(value ?? 0));
}

function AdminStatisticsPage() {
  const navigate = useNavigate();

  const [statistics, setStatistics] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");

  const loadStatistics = async () => {
    try {
      setIsLoading(true);
      setError("");

      const response = await api.get(
        "/admin/statistics"
      );

      setStatistics(response.data);
    } catch (requestError) {
      setError(
        requestError.response?.status === 403
          ? "Ova stranica je dostupna samo administratoru."
          : requestError.response?.data?.detail ||
              "Nije moguće učitati statistiku."
      );
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadStatistics();
  }, []);

  const cards = statistics
    ? [
        {
          title: "Korisnici",
          value: statistics.totalUsers,
          description:
            "Ukupan broj registrovanih korisnika",
          icon: Users,
          className: "users",
        },
        {
          title: "Vozila",
          value: statistics.totalVehicles,
          description:
            "Ukupan broj postavljenih vozila",
          icon: Car,
          className: "vehicles",
        },
        {
          title: "Rezervacije",
          value: statistics.totalReservations,
          description:
            "Sve rezervacije na platformi",
          icon: RefreshCw,
          className: "reservations",
        },
        {
          title: "Završeni najmovi",
          value: statistics.completedReservations,
          description:
            "Uspešno završene rezervacije",
          icon: CheckCircle,
          className: "completed",
        },
        {
          title: "Otvoreni sporovi",
          value: statistics.openDisputes,
          description:
            "Sporovi koji čekaju odluku",
          icon: AlertTriangle,
          className: "disputes",
        },
        {
          title: "Mock prihod",
          value: formatMoney(
            statistics.totalMockRevenue
          ),
          description:
            "Ukupan iznos evidentiranih mock uplata",
          icon: CircleDollarSign,
          className: "revenue",
        },
      ]
    : [];

  return (
    <div className="admin-statistics-page">
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

      <main className="admin-statistics-container">
        <div className="admin-statistics-heading">
          <div>
            <p className="section-label">
              ADMINISTRACIJA
            </p>

            <h1>Pregled platforme</h1>

            <p>
              Najvažniji podaci o korisnicima, vozilima,
              rezervacijama i transakcijama.
            </p>
          </div>

          <button
            type="button"
            className="refresh-statistics-button"
            onClick={loadStatistics}
            disabled={isLoading}
          >
            <RefreshCw
              size={18}
              className={
                isLoading ? "rotating" : ""
              }
            />

            Osveži podatke
          </button>
        </div>

        {isLoading && (
          <div className="status-card">
            Učitavanje statistike...
          </div>
        )}

        {error && (
          <div className="status-card error-message">
            {error}
          </div>
        )}

        {!isLoading && !error && statistics && (
          <>
            <section className="admin-statistics-grid">
              {cards.map((card) => {
                const Icon = card.icon;

                return (
                  <article
                    className={`admin-statistic-card ${card.className}`}
                    key={card.title}
                  >
                    <div className="statistic-card-icon">
                      <Icon size={24} />
                    </div>

                    <div>
                      <span>{card.title}</span>
                      <strong>{card.value}</strong>
                      <p>{card.description}</p>
                    </div>
                  </article>
                );
              })}
            </section>

            <section className="admin-shortcuts">
              <div>
                <p className="section-label">
                  BRZE RADNJE
                </p>

                <h2>Administratorske stranice</h2>
              </div>

              <div className="admin-shortcut-buttons">
                <button
                  type="button"
                  onClick={() =>
                    navigate(
                      "/admin/driver-verifications"
                    )
                  }
                >
                  <Users size={19} />
                  Provera vozača
                </button>

                <button
                  type="button"
                  onClick={() =>
                    navigate(
                      "/admin/vehicle-verifications"
                    )
                  }
                >
                  <Car size={19} />
                  Provera vozila
                </button>

                <button
                  type="button"
                  onClick={() =>
                    navigate("/admin/disputes")
                  }
                >
                  <AlertTriangle size={19} />
                  Otvoreni sporovi
                </button>
              </div>
            </section>
          </>
        )}
      </main>
    </div>
  );
}

export default AdminStatisticsPage;