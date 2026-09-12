import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  ArrowLeft,
  Car,
  Mail,
  MessageSquare,
  ShieldCheck,
  Star,
  User,
} from "lucide-react";

import api from "../api/api";
import DriverLicenseForm from "../components/DriverLicenseForm";
import "./ProfileReviews.css";

const verificationStatuses = {
  0: {
    text: "Nije poslato",
    className: "not-submitted",
  },
  1: {
    text: "Čeka proveru",
    className: "pending",
  },
  2: {
    text: "Verifikovano",
    className: "approved",
  },
  3: {
    text: "Odbijeno",
    className: "rejected",
  },
};

function formatDate(date) {
  return new Intl.DateTimeFormat("sr-RS", {
    dateStyle: "medium",
  }).format(new Date(date));
}

function ProfilePage() {
  const navigate = useNavigate();

  const [profile, setProfile] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const loadProfile = async () => {
      try {
        setError("");

        const response = await api.get("/users/me");

        setProfile(response.data);
      } catch (requestError) {
        setError(
          requestError.response?.data?.detail ||
            "Nije moguće učitati profil."
        );
      } finally {
        setIsLoading(false);
      }
    };

    loadProfile();
  }, []);

  if (isLoading) {
    return (
      <div className="details-status">
        Učitavanje profila...
      </div>
    );
  }

  if (error || !profile) {
    return (
      <div className="details-status">
        <p>{error}</p>

        <button
          type="button"
          onClick={() => navigate("/vehicles")}
        >
          Nazad
        </button>
      </div>
    );
  }

  const verification =
    verificationStatuses[
      profile.driverVerificationStatus
    ] ?? verificationStatuses[0];

  const averageRating = Number(
    profile.averageRating ?? 0
  );

  const receivedReviews =
    profile.receivedReviews ?? [];

  return (
    <div className="profile-page">
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

      <main className="profile-container">
        <section className="profile-banner">
          <div className="profile-avatar">
            {profile.firstName?.charAt(0)}
            {profile.lastName?.charAt(0)}
          </div>

          <div>
            <p className="section-label">
              CARSHARE PROFIL
            </p>

            <h1>
              {profile.firstName} {profile.lastName}
            </h1>

            <span>
              <Mail size={16} />
              {profile.email}
            </span>
          </div>
        </section>

        <section className="profile-grid">
          <article className="profile-card">
            <h2>Podaci o korisniku</h2>

            <div className="profile-data-row">
              <User size={19} />

              <span>
                <small>Ime i prezime</small>
                {profile.firstName}{" "}
                {profile.lastName}
              </span>
            </div>

            <div className="profile-data-row">
              <Mail size={19} />

              <span>
                <small>Email adresa</small>
                {profile.email}
              </span>
            </div>

            <div className="profile-data-row">
              <Car size={19} />

              <span>
                <small>Uloge</small>

                <span className="profile-roles">
                  {profile.isOwner && (
                    <b>Vlasnik</b>
                  )}

                  {profile.isRenter && (
                    <b>Renter</b>
                  )}

                  {profile.isAdmin && (
                    <b>Administrator</b>
                  )}
                </span>
              </span>
            </div>
          </article>

          <article className="profile-card">
            <h2>Reputacija</h2>

            <div className="score-grid">
              <div>
                <Star size={25} />

                <strong>
                  {averageRating > 0
                    ? averageRating.toFixed(1)
                    : "—"}
                </strong>

                <span>Prosečna ocena</span>
              </div>

              <div>
                <ShieldCheck size={25} />

                <strong>
                  {Number(
                    profile.trustScore ?? 0
                  ).toFixed(0)}
                </strong>

                <span>Trust score</span>
              </div>
            </div>
          </article>

          <article className="profile-card verification-card">
            <div>
              <h2>Vozačka dozvola</h2>

              <p>
                Status provere vozačke dozvole na
                CarShare platformi.
              </p>
            </div>

            <span
              className={`verification-badge ${verification.className}`}
            >
              {verification.text}
            </span>

            {(profile.driverVerificationStatus === 0 ||
              profile.driverVerificationStatus === 3) && (
              <DriverLicenseForm
                onSubmitted={(driverLicenseUrl) =>
                  setProfile((currentProfile) => ({
                    ...currentProfile,
                    driverLicenseUrl,
                    driverVerificationStatus: 1,
                  }))
                }
              />
            )}

            {profile.driverLicenseUrl && (
              <a
                href={profile.driverLicenseUrl}
                target="_blank"
                rel="noreferrer"
              >
                Pogledaj dokument
              </a>
            )}
          </article>
        </section>

        <section className="profile-reviews-section">
          <div className="profile-reviews-heading">
            <div>
              <p className="section-label">
                VAŠA REPUTACIJA
              </p>

              <h2>Ocene i komentari</h2>
            </div>

            <span>
              {receivedReviews.length}{" "}
              {receivedReviews.length === 1
                ? "ocena"
                : "ocena"}
            </span>
          </div>

          {receivedReviews.length === 0 ? (
            <div className="profile-reviews-empty">
              <MessageSquare size={35} />

              <h3>Još nemate komentare</h3>

              <p>
                Ocene i komentari će se pojaviti nakon
                završene rezervacije.
              </p>
            </div>
          ) : (
            <div className="profile-reviews-list">
              {receivedReviews.map((review) => (
                <article
                  className="profile-review-card"
                  key={review.reviewId}
                >
                  <div className="profile-review-top">
                    <div>
                      <strong>
                        {review.reviewerName}
                      </strong>

                      <span>
                        {formatDate(
                          review.createdAtUtc
                        )}
                      </span>
                    </div>

                    <div
                      className="profile-review-stars"
                      aria-label={`Ocena ${review.rating} od 5`}
                    >
                      {Array.from(
                        { length: 5 },
                        (_, index) => (
                          <Star
                            key={index}
                            size={18}
                            fill={
                              index < review.rating
                                ? "currentColor"
                                : "none"
                            }
                          />
                        )
                      )}
                    </div>
                  </div>

                  <p>
                    {review.comment?.trim() ||
                      "Korisnik nije ostavio komentar."}
                  </p>
                </article>
              ))}
            </div>
          )}
        </section>
      </main>
    </div>
  );
}

export default ProfilePage;