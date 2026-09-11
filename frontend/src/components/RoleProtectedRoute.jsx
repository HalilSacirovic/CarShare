import { useEffect, useState } from "react";
import { Navigate } from "react-router-dom";

import api from "../api/api";

function RoleProtectedRoute({
  requiredRole,
  children,
}) {
  const [isLoading, setIsLoading] = useState(true);
  const [isAuthorized, setIsAuthorized] =
    useState(false);
  const [shouldLogin, setShouldLogin] =
    useState(false);

  useEffect(() => {
    const checkRole = async () => {
      try {
        const response = await api.get("/users/me");

        setIsAuthorized(
          response.data[requiredRole] === true
        );
      } catch (error) {
        if (error.response?.status === 401) {
          localStorage.removeItem("accessToken");
          localStorage.removeItem("refreshToken");
          setShouldLogin(true);
        }
      } finally {
        setIsLoading(false);
      }
    };

    checkRole();
  }, [requiredRole]);

  if (isLoading) {
    return (
      <div className="details-status">
        Provera dozvole...
      </div>
    );
  }

  if (shouldLogin) {
    return <Navigate to="/login" replace />;
  }

  if (!isAuthorized) {
    return <Navigate to="/vehicles" replace />;
  }

  return children;
}

export default RoleProtectedRoute;