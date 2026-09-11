import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import {
  ArrowLeft,
  Car,
  MapPin,
  Save,
} from "lucide-react";

import api from "../api/api";
import "./EditVehiclePage.css";

const initialForm = {
  brand: "",
  model: "",
  year: "",
  color: "",
  mileage: "",
  seatCount: "",
  transmissionType: 0,
  fuelType: 0,
  description: "",
  address: "",
  latitude: 0,
  longitude: 0,
  pricePerHour: "",
  pricePerDay: "",
  depositAmount: "",
  smokingAllowed: false,
  petsAllowed: false,
  dailyMileageLimit: "",
  autoApproveReservations: false,
};

async function findCoordinates(address) {
  const query = encodeURIComponent(address);

  const response = await fetch(
    `https://nominatim.openstreetmap.org/search?format=json&limit=1&countrycodes=rs&q=${query}`,
    {
      headers: {
        Accept: "application/json",
      },
    }
  );

  if (!response.ok) {
    throw new Error("Pretraga adrese nije uspela.");
  }

  const results = await response.json();

  if (!results.length) {
    throw new Error(
      "Adresa nije pronađena. Unesite grad i precizniju adresu."
    );
  }

  return {
    latitude: Number(results[0].lat),
    longitude: Number(results[0].lon),
  };
}

function EditVehiclePage() {
  const { vehicleId } = useParams();
  const navigate = useNavigate();

  const [form, setForm] = useState(initialForm);
  const [originalAddress, setOriginalAddress] =
    useState("");
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState("");
  const [message, setMessage] = useState("");

  useEffect(() => {
    const loadVehicle = async () => {
      try {
        setError("");

        const response = await api.get(
          `/vehicles/${vehicleId}`
        );

        const vehicle = response.data;

        setForm({
          brand: vehicle.brand ?? "",
          model: vehicle.model ?? "",
          year: vehicle.year ?? "",
          color: vehicle.color ?? "",
          mileage: vehicle.mileage ?? "",
          seatCount: vehicle.seatCount ?? "",
          transmissionType:
            vehicle.transmissionType ?? 0,
          fuelType: vehicle.fuelType ?? 0,
          description: vehicle.description ?? "",
          address: vehicle.address ?? "",
          latitude: vehicle.latitude ?? 0,
          longitude: vehicle.longitude ?? 0,
          pricePerHour: vehicle.pricePerHour ?? "",
          pricePerDay: vehicle.pricePerDay ?? "",
          depositAmount:
            vehicle.depositAmount ?? "",
          smokingAllowed:
            vehicle.smokingAllowed ?? false,
          petsAllowed: vehicle.petsAllowed ?? false,
          dailyMileageLimit:
            vehicle.dailyMileageLimit ?? "",
          autoApproveReservations:
            vehicle.autoApproveReservations ?? false,
        });

        setOriginalAddress(vehicle.address ?? "");
      } catch (requestError) {
        setError(
          requestError.response?.data?.detail ||
            "Vozilo nije moguće učitati."
        );
      } finally {
        setIsLoading(false);
      }
    };

    loadVehicle();
  }, [vehicleId]);

  const updateField = (event) => {
    const { name, value, type, checked } =
      event.target;

    setForm((currentForm) => ({
      ...currentForm,
      [name]: type === "checkbox" ? checked : value,
    }));
  };

  const submitForm = async (event) => {
    event.preventDefault();

    try {
      setIsSaving(true);
      setError("");
      setMessage("");

      let latitude = Number(form.latitude);
      let longitude = Number(form.longitude);

      if (
        form.address.trim().toLowerCase() !==
          originalAddress.trim().toLowerCase() ||
        !latitude ||
        !longitude
      ) {
        const coordinates = await findCoordinates(
          form.address.trim()
        );

        latitude = coordinates.latitude;
        longitude = coordinates.longitude;
      }

      const requestBody = {
        brand: form.brand.trim(),
        model: form.model.trim(),
        year: Number(form.year),
        color: form.color.trim(),
        mileage: Number(form.mileage),
        seatCount: Number(form.seatCount),
        transmissionType: Number(
          form.transmissionType
        ),
        fuelType: Number(form.fuelType),
        description: form.description.trim(),
        address: form.address.trim(),
        latitude,
        longitude,
        pricePerHour:
          form.pricePerHour === ""
            ? null
            : Number(form.pricePerHour),
        pricePerDay:
          form.pricePerDay === ""
            ? null
            : Number(form.pricePerDay),
        depositAmount: Number(
          form.depositAmount
        ),
        smokingAllowed: form.smokingAllowed,
        petsAllowed: form.petsAllowed,
        dailyMileageLimit: Number(
          form.dailyMileageLimit
        ),
        autoApproveReservations:
          form.autoApproveReservations,
      };

      await api.put(
        `/vehicles/${vehicleId}`,
        requestBody
      );

      setMessage("Izmene su uspešno sačuvane.");
      setOriginalAddress(form.address.trim());

      setForm((currentForm) => ({
        ...currentForm,
        latitude,
        longitude,
      }));

      setTimeout(() => {
        navigate("/owner/vehicles");
      }, 1000);
    } catch (requestError) {
      setError(
        requestError.response?.data?.detail ||
          requestError.message ||
          "Izmene nije moguće sačuvati."
      );
    } finally {
      setIsSaving(false);
    }
  };

  if (isLoading) {
    return (
      <div className="details-status">
        Učitavanje vozila...
      </div>
    );
  }

  return (
    <div className="edit-vehicle-page">
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
            navigate("/owner/vehicles")
          }
        >
          <ArrowLeft size={18} />
          Nazad na moja vozila
        </button>
      </header>

      <main className="edit-vehicle-container">
        <div className="edit-vehicle-heading">
          <div className="edit-vehicle-icon">
            <Car size={27} />
          </div>

          <div>
            <p className="section-label">
              VLASNIČKI PANEL
            </p>

            <h1>Izmeni vozilo</h1>

            <p>
              Ažurirajte podatke, lokaciju i cenu
              iznajmljivanja.
            </p>
          </div>
        </div>

        {error && (
          <div className="status-card error-message">
            {error}
          </div>
        )}

        {message && (
          <div className="edit-vehicle-success">
            {message}
          </div>
        )}

        {!error && (
          <form
            className="edit-vehicle-form"
            onSubmit={submitForm}
          >
            <section className="edit-form-section">
              <h2>Osnovni podaci</h2>

              <div className="edit-form-grid">
                <label>
                  Marka
                  <input
                    name="brand"
                    value={form.brand}
                    onChange={updateField}
                    required
                  />
                </label>

                <label>
                  Model
                  <input
                    name="model"
                    value={form.model}
                    onChange={updateField}
                    required
                  />
                </label>

                <label>
                  Godina proizvodnje
                  <input
                    name="year"
                    type="number"
                    min="1950"
                    max="2030"
                    value={form.year}
                    onChange={updateField}
                    required
                  />
                </label>

                <label>
                  Boja
                  <input
                    name="color"
                    value={form.color}
                    onChange={updateField}
                    required
                  />
                </label>

                <label>
                  Kilometraža
                  <input
                    name="mileage"
                    type="number"
                    min="0"
                    value={form.mileage}
                    onChange={updateField}
                    required
                  />
                </label>

                <label>
                  Broj sedišta
                  <input
                    name="seatCount"
                    type="number"
                    min="1"
                    max="20"
                    value={form.seatCount}
                    onChange={updateField}
                    required
                  />
                </label>

                <label>
                  Menjač
                  <select
                    name="transmissionType"
                    value={form.transmissionType}
                    onChange={updateField}
                  >
                    <option value={0}>Manuelni</option>
                    <option value={1}>Automatski</option>
                  </select>
                </label>

                <label>
                  Gorivo
                  <select
                    name="fuelType"
                    value={form.fuelType}
                    onChange={updateField}
                  >
                    <option value={0}>Benzin</option>
                    <option value={1}>Dizel</option>
                    <option value={2}>
                      Električno
                    </option>
                    <option value={3}>Hibrid</option>
                  </select>
                </label>
              </div>
            </section>

            <section className="edit-form-section">
              <h2>Lokacija i opis</h2>

              <label className="full-field">
                <span>
                  <MapPin size={17} />
                  Grad i adresa
                </span>

                <input
                  name="address"
                  value={form.address}
                  onChange={updateField}
                  placeholder="Na primer: Novi Pazar, Srbija"
                  required
                />

                <small>
                  Koordinate će biti pronađene automatski.
                </small>
              </label>

              <label className="full-field">
                Opis vozila

                <textarea
                  name="description"
                  value={form.description}
                  onChange={updateField}
                  minLength={10}
                  maxLength={1000}
                  required
                />
              </label>
            </section>

            <section className="edit-form-section">
              <h2>Cene i pravila</h2>

              <div className="edit-form-grid">
                <label>
                  Cena po satu (€)
                  <input
                    name="pricePerHour"
                    type="number"
                    min="0"
                    step="0.01"
                    value={form.pricePerHour}
                    onChange={updateField}
                  />
                </label>

                <label>
                  Cena po danu (€)
                  <input
                    name="pricePerDay"
                    type="number"
                    min="0"
                    step="0.01"
                    value={form.pricePerDay}
                    onChange={updateField}
                  />
                </label>

                <label>
                  Depozit (€)
                  <input
                    name="depositAmount"
                    type="number"
                    min="0"
                    step="0.01"
                    value={form.depositAmount}
                    onChange={updateField}
                    required
                  />
                </label>

                <label>
                  Dnevni limit kilometara
                  <input
                    name="dailyMileageLimit"
                    type="number"
                    min="0"
                    value={form.dailyMileageLimit}
                    onChange={updateField}
                    required
                  />
                </label>
              </div>

              <div className="edit-checkbox-grid">
                <label>
                  <input
                    name="smokingAllowed"
                    type="checkbox"
                    checked={form.smokingAllowed}
                    onChange={updateField}
                  />
                  Pušenje je dozvoljeno
                </label>

                <label>
                  <input
                    name="petsAllowed"
                    type="checkbox"
                    checked={form.petsAllowed}
                    onChange={updateField}
                  />
                  Kućni ljubimci su dozvoljeni
                </label>

                <label>
                  <input
                    name="autoApproveReservations"
                    type="checkbox"
                    checked={
                      form.autoApproveReservations
                    }
                    onChange={updateField}
                  />
                  Automatski odobri rezervacije
                </label>
              </div>
            </section>

            <div className="edit-form-actions">
              <button
                type="button"
                className="edit-cancel-button"
                onClick={() =>
                  navigate("/owner/vehicles")
                }
              >
                Odustani
              </button>

              <button
                type="submit"
                className="edit-save-button"
                disabled={isSaving}
              >
                <Save size={18} />

                {isSaving
                  ? "Čuvanje..."
                  : "Sačuvaj izmene"}
              </button>
            </div>
          </form>
        )}
      </main>
    </div>
  );
}

export default EditVehiclePage;