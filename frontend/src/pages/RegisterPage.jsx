import { useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../api/api";

function RegisterPage() {
  const navigate = useNavigate();

  const [form, setForm] = useState({
    firstName: "",
    lastName: "",
    email: "",
    password: "",
    isOwner: false,
    isRenter: true,
  });

  const [message, setMessage] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  const handleChange = (event) => {
    const { name, value, type, checked } = event.target;

    setForm((current) => ({
      ...current,
      [name]: type === "checkbox" ? checked : value,
    }));
  };

  const handleSubmit = async (event) => {
    event.preventDefault();

    try {
      setIsLoading(true);
      setMessage("");

      const response = await api.post(
        "/auth/register",
        form
      );

      if (response.data.accessToken) {
        localStorage.setItem(
          "accessToken",
          response.data.accessToken
        );

        localStorage.setItem(
          "refreshToken",
          response.data.refreshToken
        );

        navigate("/vehicles");
        return;
      }

      navigate("/login");
    } catch (error) {
      const validationErrors =
        error.response?.data?.errors;

      if (validationErrors) {
        const firstError =
          Object.values(validationErrors)[0]?.[0];

        setMessage(
          firstError ?? "Proverite unete podatke."
        );
      } else {
        setMessage(
          error.response?.data?.detail ??
            "Registracija nije uspela."
        );
      }
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <main className="register-page">
      <section className="register-card">
        <button
          className="register-brand"
          onClick={() => navigate("/login")}
        >
          <span className="brand-mark">◆</span>
          CarShare
        </button>

        <p className="section-label">POSTANITE ČLAN</p>
        <h1>Kreirajte nalog</h1>

        <p className="subtitle">
          Pridružite se zajednici vlasnika i putnika.
        </p>

        <form onSubmit={handleSubmit}>
          <div className="register-name-grid">
            <div>
              <label htmlFor="firstName">Ime</label>

              <input
                id="firstName"
                name="firstName"
                value={form.firstName}
                onChange={handleChange}
                required
              />
            </div>

            <div>
              <label htmlFor="lastName">Prezime</label>

              <input
                id="lastName"
                name="lastName"
                value={form.lastName}
                onChange={handleChange}
                required
              />
            </div>
          </div>

          <label htmlFor="registerEmail">
            Email adresa
          </label>

          <input
            id="registerEmail"
            name="email"
            type="email"
            value={form.email}
            onChange={handleChange}
            required
          />

          <label htmlFor="registerPassword">
            Lozinka
          </label>

          <input
            id="registerPassword"
            name="password"
            type="password"
            value={form.password}
            onChange={handleChange}
            minLength={8}
            required
          />

          <label className="owner-option">
            <input
              name="isOwner"
              type="checkbox"
              checked={form.isOwner}
              onChange={handleChange}
            />

            <span>
              <strong>Želim da izdajem vozilo</strong>
              Moći ćete da dodajete svoja vozila.
            </span>
          </label>

          <button
            className="register-submit"
            type="submit"
            disabled={isLoading}
          >
            {isLoading
              ? "Kreiranje naloga..."
              : "Napravi nalog"}
          </button>
        </form>

        {message && (
          <p className="register-error">{message}</p>
        )}

        <p className="login-link">
          Već imate nalog?

          <button onClick={() => navigate("/login")}>
            Prijavite se
          </button>
        </p>
      </section>
    </main>
  );
}

export default RegisterPage;