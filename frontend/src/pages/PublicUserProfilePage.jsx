import { useEffect, useState } from "react";
import {
  ArrowLeft,
  BadgeCheck,
  Car,
  MessageSquare,
  ShieldCheck,
  Star,
} from "lucide-react";
import {
  useNavigate,
  useParams,
} from "react-router-dom";

import api from "../api/api";
import "./PublicUserProfilePage.css";

function formatDate(date) {
  if (!date) {
    return "";
  }

  return new Intl.DateTimeFormat("sr-RS", {
    dateStyle: "medium",
  }).format(new Date(date));
}

function PublicUserProfilePage() {
  const { userId } = useParams();
  const navigate = useNavigate();

  const [profile, setProfile] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const loadProfile = async () => {
      try {
        setError("");

        const response = await api.get(
          `/users/${userId}/public-profile`
        );

        setProfile(response.data);
      } catch (requestError) {
        setError(
          requestError.response?.data?.detail ||
            "Nije moguće učitati profil korisnika."
        );
      } finally {
        setIsLoading(false);
      }
    };

    loadProfile();
  }, [userId]);

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
          onClick={() =>
            navigate("/owner/reservations")
          }
        >
          Nazad na rezervacije
        </button>
      </div>
    );
  }

  const averageRating = Number(
    profile.averageRating ?? 0
  );

  const trustScore = Number(
    profile.trustScore ?? 0
  );

  return (
    <div className="public-profile-page">
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
          onClick={() =>
            navigate("/owner/reservations")
          }
        >
          <ArrowLeft size={18} />
          Nazad na zahteve
        </button>
      </header>

      <main className="public-profile-container">
        <section className="public-profile-card">
          <div className="public-profile-avatar">
            {profile.fullName
              ?.charAt(0)
              .toUpperCase()}
          </div>

          <div className="public-profile-identity">
            <p className="section-label">
              PROFIL RENTER-A
            </p>

            <h1>{profile.fullName}</h1>

            {profile.isDriverVerified ? (
              <span className="driver-status verified">
                <BadgeCheck size={18} />
                Vozačka dozvola je verifikovana
              </span>
            ) : (
              <span className="driver-status unverified">
                Vozačka dozvola nije verifikovana
              </span>
            )}
          </div>
        </section>

        <section className="public-profile-statistics">
          <article>
            <Star size={24} />
            <span>Prosečna ocena</span>
            <strong>
              {averageRating > 0
                ? averageRating.toFixed(1)
                : "Nema ocena"}
            </strong>
          </article>

          <article>
            <ShieldCheck size={24} />
            <span>Trust score</span>
            <strong>
              {trustScore.toFixed(0)} / 100
            </strong>
          </article>

          <article>
            <Car size={24} />
            <span>Završene rezervacije</span>
            <strong>
              {profile.completedReservations}
            </strong>
          </article>
        </section>

        <section className="public-reviews-section">
          <div className="public-reviews-heading">
            <div>
              <p className="section-label">
                ISKUSTVA VLASNIKA
              </p>

              <h2>Ocene i komentari</h2>
            </div>

            <span>
              {profile.reviews.length}{" "}
              {profile.reviews.length === 1
                ? "ocena"
                : "ocena"}
            </span>
          </div>

          {profile.reviews.length === 0 ? (
            <div className="public-reviews-empty">
              <MessageSquare size={34} />

              <h3>Još nema komentara</h3>

              <p>
                Ovaj korisnik još uvek nije dobio
                nijednu ocenu.
              </p>
            </div>
          ) : (
            <div className="public-reviews-list">
              {profile.reviews.map((review) => (
                <article
                  className="public-review-card"
                  key={review.reviewId}
                >
                  <div className="public-review-top">
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
                      className="public-review-stars"
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

export default PublicUserProfilePage;