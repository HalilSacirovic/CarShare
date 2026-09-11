import { useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../api/api";

function LoginPage() {
  const navigate = useNavigate();

  const [email, setEmail] = useState("halil@example.com");
  const [password, setPassword] = useState("Test123!");
  const [message, setMessage] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (event) => {
    event.preventDefault();

    try {
      setIsLoading(true);
      setMessage("");

      const response = await api.post("/auth/login", {
        email,
        password,
      });

      localStorage.setItem(
        "accessToken",
        response.data.accessToken
      );

      localStorage.setItem(
        "refreshToken",
        response.data.refreshToken
      );

      navigate("/vehicles");
    } catch (error) {
      setMessage(
        error.response?.data?.detail ??
          "Prijavljivanje nije uspelo."
      );
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <main className="login-page">
      <section className="login-card">
        <div className="logo">CarShare</div>

        <h1>Dobro došli</h1>

        <p className="subtitle">
          Prijavite se i pronađite automobil za svoje putovanje.
        </p>

        <form onSubmit={handleSubmit}>
          <label htmlFor="email">Email adresa</label>

          <input
            id="email"
            type="email"
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            required
          />

          <label htmlFor="password">Lozinka</label>

          <input
            id="password"
            type="password"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            required
          />

          <button type="submit" disabled={isLoading}>
            {isLoading ? "Prijavljivanje..." : "Prijavi se"}
          </button>

          <p className="login-link">
            Nemate nalog?

            <button
                type="button"
                onClick={() => navigate("/register")}
            >
                Registrujte se
            </button>
            </p>
        </form>

        {message && <p className="message">{message}</p>}
      </section>
    </main>
  );
}

export default LoginPage;