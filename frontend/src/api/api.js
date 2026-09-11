import axios from "axios";

const API_BASE_URL = "http://localhost:8080/api/v1";

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

const refreshClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

let refreshRequest = null;

function clearAuthentication() {
  localStorage.removeItem("accessToken");
  localStorage.removeItem("refreshToken");
  localStorage.removeItem("accessTokenExpiresAtUtc");
  localStorage.removeItem("refreshTokenExpiresAtUtc");
}

function redirectToLogin() {
  if (window.location.pathname !== "/login") {
    window.location.replace("/login");
  }
}

api.interceptors.request.use(
  (config) => {
    const accessToken = localStorage.getItem("accessToken");

    if (accessToken) {
      config.headers.Authorization = `Bearer ${accessToken}`;
    }

    return config;
  },
  (error) => Promise.reject(error),
);

api.interceptors.response.use(
  (response) => response,

  async (error) => {
    const originalRequest = error.config;

    const isUnauthorized = error.response?.status === 401;

    if (!isUnauthorized || !originalRequest || originalRequest._retry) {
      return Promise.reject(error);
    }

    const refreshToken = localStorage.getItem("refreshToken");

    if (!refreshToken) {
      clearAuthentication();
      redirectToLogin();

      return Promise.reject(error);
    }

    originalRequest._retry = true;

    try {
      if (!refreshRequest) {
        refreshRequest = refreshClient
          .post("/auth/refresh", {
            refreshToken,
          })
          .then((response) => {
            const authResult = response.data;

            localStorage.setItem("accessToken", authResult.accessToken);

            localStorage.setItem("refreshToken", authResult.refreshToken);

            localStorage.setItem(
              "accessTokenExpiresAtUtc",
              authResult.accessTokenExpiresAtUtc,
            );

            localStorage.setItem(
              "refreshTokenExpiresAtUtc",
              authResult.refreshTokenExpiresAtUtc,
            );

            return authResult.accessToken;
          })
          .finally(() => {
            refreshRequest = null;
          });
      }

      const newAccessToken = await refreshRequest;

      originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;

      return api(originalRequest);
    } catch (refreshError) {
      clearAuthentication();
      redirectToLogin();

      return Promise.reject(refreshError);
    }
  },
);

export default api;
