import {
  BrowserRouter,
  Navigate,
  Route,
  Routes,
} from "react-router-dom";

import "./App.css";

import RoleProtectedRoute from "./components/RoleProtectedRoute";

import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import VehiclesPage from "./pages/VehiclesPage";
import VehicleDetailsPage from "./pages/VehicleDetailsPage";
import CreateVehiclePage from "./pages/CreateVehiclePage";
import EditVehiclePage from "./pages/EditVehiclePage";
import MyVehiclesPage from "./pages/MyVehiclesPage";
import VehicleAvailabilityPage from "./pages/VehicleAvailabilityPage";
import MyReservationsPage from "./pages/MyReservationsPage";
import OwnerReservationsPage from "./pages/OwnerReservationsPage";
import ProfilePage from "./pages/ProfilePage";
import AdminVehicleVerificationsPage from "./pages/AdminVehicleVerificationsPage";
import AdminDriverVerificationsPage from "./pages/AdminDriverVerificationsPage";
import AdminDisputesPage from "./pages/AdminDisputesPage";
import AdminStatisticsPage from "./pages/AdminStatisticsPage";

function App() {
  const isLoggedIn = Boolean(
    localStorage.getItem("accessToken")
  );

  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginPage />} />

        <Route
          path="/register"
          element={<RegisterPage />}
        />

        <Route
          path="/vehicles"
          element={
            isLoggedIn ? (
              <VehiclesPage />
            ) : (
              <Navigate to="/login" replace />
            )
          }
        />

        <Route
          path="/vehicles/create"
          element={
            isLoggedIn ? (
              <RoleProtectedRoute requiredRole="isOwner">
                <CreateVehiclePage />
              </RoleProtectedRoute>
            ) : (
              <Navigate to="/login" replace />
            )
          }
        />

        <Route
          path="/vehicles/:vehicleId"
          element={
            isLoggedIn ? (
              <VehicleDetailsPage />
            ) : (
              <Navigate to="/login" replace />
            )
          }
        />

        <Route
          path="/reservations"
          element={
            isLoggedIn ? (
              <MyReservationsPage />
            ) : (
              <Navigate to="/login" replace />
            )
          }
        />

        <Route
          path="/profile"
          element={
            isLoggedIn ? (
              <ProfilePage />
            ) : (
              <Navigate to="/login" replace />
            )
          }
        />

        <Route
          path="/owner/vehicles"
          element={
            isLoggedIn ? (
              <RoleProtectedRoute requiredRole="isOwner">
                <MyVehiclesPage />
              </RoleProtectedRoute>
            ) : (
              <Navigate to="/login" replace />
            )
          }
        />

        <Route
          path="/owner/vehicles/:vehicleId/edit"
          element={
            isLoggedIn ? (
              <RoleProtectedRoute requiredRole="isOwner">
                <EditVehiclePage />
              </RoleProtectedRoute>
            ) : (
              <Navigate to="/login" replace />
            )
          }
        />

        <Route
          path="/owner/vehicles/:vehicleId/availability"
          element={
            isLoggedIn ? (
              <RoleProtectedRoute requiredRole="isOwner">
                <VehicleAvailabilityPage />
              </RoleProtectedRoute>
            ) : (
              <Navigate to="/login" replace />
            )
          }
        />

        <Route
          path="/owner/reservations"
          element={
            isLoggedIn ? (
              <RoleProtectedRoute requiredRole="isOwner">
                <OwnerReservationsPage />
              </RoleProtectedRoute>
            ) : (
              <Navigate to="/login" replace />
            )
          }
        />

        <Route
          path="/admin/statistics"
          element={
            isLoggedIn ? (
              <RoleProtectedRoute requiredRole="isAdmin">
                <AdminStatisticsPage />
              </RoleProtectedRoute>
            ) : (
              <Navigate to="/login" replace />
            )
          }
        />

        <Route
          path="/admin/vehicle-verifications"
          element={
            isLoggedIn ? (
              <RoleProtectedRoute requiredRole="isAdmin">
                <AdminVehicleVerificationsPage />
              </RoleProtectedRoute>
            ) : (
              <Navigate to="/login" replace />
            )
          }
        />

        <Route
          path="/admin/driver-verifications"
          element={
            isLoggedIn ? (
              <RoleProtectedRoute requiredRole="isAdmin">
                <AdminDriverVerificationsPage />
              </RoleProtectedRoute>
            ) : (
              <Navigate to="/login" replace />
            )
          }
        />

        <Route
          path="/admin/disputes"
          element={
            isLoggedIn ? (
              <RoleProtectedRoute requiredRole="isAdmin">
                <AdminDisputesPage />
              </RoleProtectedRoute>
            ) : (
              <Navigate to="/login" replace />
            )
          }
        />

        <Route
          path="*"
          element={
            <Navigate
              to={
                isLoggedIn
                  ? "/vehicles"
                  : "/login"
              }
              replace
            />
          }
        />
      </Routes>
    </BrowserRouter>
  );
}

export default App;