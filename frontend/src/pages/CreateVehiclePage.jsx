import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  ArrowLeft,
  Car,
  Image,
  Trash2,
} from "lucide-react";

import api from "../api/api";
import FileUploadField from "../components/FileUploadField";
import { findSerbianLocationCoordinates } from "../services/geocodingService";

const initialForm = {
  brand: "",
  model: "",
  year: 2020,
  color: "",
  mileage: 0,
  seatCount: 5,
  transmissionType: 0,
  fuelType: 0,
  description: "",
  address: "Novi Pazar, Srbija",
  pricePerHour: "",
  pricePerDay: 40,
  depositAmount: 100,
  smokingAllowed: false,
  petsAllowed: false,
  dailyMileageLimit: 300,
  autoApproveReservations: false,
};

function CreateVehiclePage() {
  const navigate = useNavigate();

  const [ownerId, setOwnerId] = useState("");
  const [form, setForm] = useState(initialForm);
  const [photoUrls, setPhotoUrls] = useState([]);
  const [message, setMessage] = useState("");
  const [isLoading, setIsLoading] =
    useState(false);

  useEffect(() => {
    const loadProfile = async () => {
      try {
        const response = await api.get("/users/me");

        setOwnerId(response.data.id);
      } catch (requestError) {
        setMessage(
          requestError.response?.data?.detail ||
            "Nije moguće učitati korisnički profil."
        );
      }
    };

    loadProfile();
  }, []);

  const handleChange = (event) => {
    const {
      name,
      value,
      type,
      checked,
    } = event.target;

    setForm((currentForm) => ({
      ...currentForm,
      [name]:
        type === "checkbox" ? checked : value,
    }));
  };

  const addPhoto = (url) => {
    setPhotoUrls((currentUrls) => {
      if (currentUrls.includes(url)) {
        return currentUrls;
      }

      return [...currentUrls, url];
    });
  };

  const removePhoto = (urlToRemove) => {
    setPhotoUrls((currentUrls) =>
      currentUrls.filter(
        (url) => url !== urlToRemove
      )
    );
  };

  const handleSubmit = async (event) => {
    event.preventDefault();

    if (photoUrls.length === 0) {
      setMessage(
        "Dodajte najmanje jednu fotografiju vozila."
      );
      return;
    }

    try {
      setIsLoading(true);
      setMessage("");

      const coordinates =
        await findSerbianLocationCoordinates(
          form.address
        );

      if (!coordinates) {
        setMessage(
          "Adresa nije pronađena. Unesite ulicu, broj i grad."
        );
        return;
      }

      const response = await api.post("/vehicles", {
        ownerId,
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
        address: coordinates.displayName,
        latitude: coordinates.latitude,
        longitude: coordinates.longitude,
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
        photoUrls,
        equipmentIds: [],
      });

      const vehicleId =
        response.data?.id ?? response.data;

      navigate(`/vehicles/${vehicleId}`);
    } catch (requestError) {
      const validationErrors =
        requestError.response?.data?.errors;

      if (validationErrors) {
        setMessage(
          Object.values(
            validationErrors
          )[0]?.[0] ||
            "Proverite unete podatke."
        );
      } else {
        setMessage(
          requestError.response?.data?.detail ||
            requestError.message ||
            "Vozilo nije moguće kreirati."
        );
      }
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="vehicle-form-page">
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
          Nazad
        </button>
      </header>

      <main className="vehicle-form-container">
        <div className="vehicle-form-heading">
          <span className="vehicle-form-icon">
            <Car size={28} />
          </span>

          <div>
            <p className="section-label">
              POSTANITE VLASNIK
            </p>

            <h1>Postavite svoje vozilo</h1>

            <p>
              Unesite osnovne podatke, cenu i uslove
              najma.
            </p>
          </div>
        </div>

        <form
          className="vehicle-form"
          onSubmit={handleSubmit}
        >
          <section>
            <h2>Osnovni podaci</h2>

            <div className="vehicle-form-grid">
              <label>
                Marka
                <input
                  name="brand"
                  value={form.brand}
                  onChange={handleChange}
                  placeholder="Ford"
                  required
                />
              </label>

              <label>
                Model
                <input
                  name="model"
                  value={form.model}
                  onChange={handleChange}
                  placeholder="Focus"
                  required
                />
              </label>

              <label>
                Godina proizvodnje
                <input
                  name="year"
                  type="number"
                  min="1990"
                  max="2026"
                  value={form.year}
                  onChange={handleChange}
                  required
                />
              </label>

              <label>
                Boja
                <input
                  name="color"
                  value={form.color}
                  onChange={handleChange}
                  placeholder="Crna"
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
                  onChange={handleChange}
                  required
                />
              </label>

              <label>
                Broj sedišta
                <input
                  name="seatCount"
                  type="number"
                  min="1"
                  max="9"
                  value={form.seatCount}
                  onChange={handleChange}
                  required
                />
              </label>

              <label>
                Menjač
                <select
                  name="transmissionType"
                  value={form.transmissionType}
                  onChange={handleChange}
                >
                  <option value="0">Manuelni</option>
                  <option value="1">
                    Automatski
                  </option>
                </select>
              </label>

              <label>
                Gorivo
                <select
                  name="fuelType"
                  value={form.fuelType}
                  onChange={handleChange}
                >
                  <option value="0">Benzin</option>
                  <option value="1">Dizel</option>
                  <option value="2">
                    Električno
                  </option>
                  <option value="3">Hibrid</option>
                </select>
              </label>
            </div>

            <label>
              Opis
              <textarea
                name="description"
                value={form.description}
                onChange={handleChange}
                placeholder="Opišite stanje i karakteristike vozila..."
                minLength="10"
                maxLength="1000"
                required
              />
            </label>
          </section>

          <section>
            <h2>Lokacija i cena</h2>

            <div className="vehicle-form-grid">
              <label className="wide-field">
                Adresa vozila

                <input
                  name="address"
                  value={form.address}
                  onChange={handleChange}
                  placeholder="Stevana Nemanje 25, Novi Pazar"
                  required
                />

                <small className="location-help">
                  Unesite ulicu, broj i grad. Koordinate
                  će biti određene automatski pomoću
                  OpenStreetMap podataka.
                </small>
              </label>

              <label>
                Cena po satu (€)
                <input
                  name="pricePerHour"
                  type="number"
                  min="0"
                  step="0.01"
                  value={form.pricePerHour}
                  onChange={handleChange}
                />
              </label>

              <label>
                Cena po danu (€)
                <input
                  name="pricePerDay"
                  type="number"
                  min="1"
                  step="0.01"
                  value={form.pricePerDay}
                  onChange={handleChange}
                  required
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
                  onChange={handleChange}
                  required
                />
              </label>

              <label>
                Dnevno ograničenje (km)
                <input
                  name="dailyMileageLimit"
                  type="number"
                  min="1"
                  value={form.dailyMileageLimit}
                  onChange={handleChange}
                  required
                />
              </label>
            </div>
          </section>

          <section>
            <h2>Fotografije vozila</h2>

            <p>
              Dodajte jednu ili više fotografija sa
              računara. Prva fotografija biće prikazana
              kao glavna.
            </p>

            <FileUploadField
              key={photoUrls.length}
              label="Dodajte fotografiju"
              accept=".jpg,.jpeg,.png,.webp"
              onUploaded={addPhoto}
            />

            {photoUrls.length > 0 && (
              <div className="uploaded-vehicle-photos">
                {photoUrls.map((url, index) => (
                  <div
                    className="uploaded-vehicle-photo"
                    key={url}
                  >
                    <img
                      src={url}
                      alt={`Fotografija vozila ${
                        index + 1
                      }`}
                    />

                    <span>
                      <Image size={16} />
                      Fotografija {index + 1}
                    </span>

                    <button
                      type="button"
                      onClick={() =>
                        removePhoto(url)
                      }
                    >
                      <Trash2 size={16} />
                      Ukloni
                    </button>
                  </div>
                ))}
              </div>
            )}
          </section>

          <section>
            <h2>Pravila najma</h2>

            <div className="vehicle-checkboxes">
              <label>
                <input
                  name="smokingAllowed"
                  type="checkbox"
                  checked={form.smokingAllowed}
                  onChange={handleChange}
                />
                Pušenje je dozvoljeno
              </label>

              <label>
                <input
                  name="petsAllowed"
                  type="checkbox"
                  checked={form.petsAllowed}
                  onChange={handleChange}
                />
                Ljubimci su dozvoljeni
              </label>

              <label>
                <input
                  name="autoApproveReservations"
                  type="checkbox"
                  checked={
                    form.autoApproveReservations
                  }
                  onChange={handleChange}
                />
                Automatski odobri rezervacije
              </label>
            </div>
          </section>

          {message && (
            <div className="reservation-message error">
              {message}
            </div>
          )}

          <button
            className="create-vehicle-button"
            type="submit"
            disabled={isLoading || !ownerId}
          >
            {isLoading
              ? "Kreiranje vozila..."
              : "Postavi vozilo"}
          </button>
        </form>
      </main>
    </div>
  );
}

export default CreateVehiclePage;