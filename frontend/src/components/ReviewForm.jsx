import { useState } from "react";
import { CheckCircle, Star } from "lucide-react";

import api from "../api/api";
import "./ReviewForm.css";

export default function ReviewForm({ reservationId }) {
  const [rating, setRating] = useState(0);
  const [hoveredRating, setHoveredRating] = useState(0);
  const [comment, setComment] = useState("");
  const [message, setMessage] = useState("");
  const [isError, setIsError] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [reviewed, setReviewed] = useState(false);

  const submitReview = async (event) => {
    event.preventDefault();

    if (rating < 1 || rating > 5) {
      setIsError(true);
      setMessage("Izaberite ocenu od 1 do 5.");
      return;
    }

    try {
      setIsLoading(true);
      setMessage("");
      setIsError(false);

      await api.post(
        `/reservations/${reservationId}/reviews`,
        {
          rating,
          comment: comment.trim() || null,
        }
      );

      setReviewed(true);
      setMessage("Ocena je uspešno sačuvana.");
    } catch (error) {
      setIsError(true);
      setMessage(
        error.response?.data?.detail ||
          "Ocenu nije moguće sačuvati."
      );
    } finally {
      setIsLoading(false);
    }
  };

  if (reviewed) {
    return (
      <div className="review-success">
        <CheckCircle size={18} />
        Hvala na oceni!
      </div>
    );
  }

  return (
    <form className="review-form" onSubmit={submitReview}>
      <div>
        <h3>Ocenite iskustvo</h3>
        <p>
          Ocenite drugu stranu nakon završenog najma.
        </p>
      </div>

      <div
        className="review-stars"
        onMouseLeave={() => setHoveredRating(0)}
      >
        {[1, 2, 3, 4, 5].map((star) => {
          const selected =
            star <= (hoveredRating || rating);

          return (
            <button
              type="button"
              className={selected ? "selected" : ""}
              key={star}
              onMouseEnter={() =>
                setHoveredRating(star)
              }
              onClick={() => {
                setRating(star);
                setMessage("");
              }}
              aria-label={`Ocena ${star}`}
            >
              <Star
                size={27}
                fill={selected ? "currentColor" : "none"}
              />
            </button>
          );
        })}
      </div>

      <span className="selected-rating">
        {rating === 0
          ? "Izaberite ocenu"
          : `Izabrana ocena: ${rating}/5`}
      </span>

      <textarea
        value={comment}
        onChange={(event) =>
          setComment(event.target.value)
        }
        minLength={3}
        maxLength={500}
        placeholder="Napišite komentar (opciono)"
      />

      <button
        className="submit-review-button"
        type="submit"
        disabled={isLoading}
      >
        {isLoading ? "Čuvanje..." : "Pošalji ocenu"}
      </button>

      {message && (
        <div
          className={
            isError
              ? "review-message error"
              : "review-message success"
          }
        >
          {message}
        </div>
      )}
    </form>
  );
}