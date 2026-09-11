import { useEffect, useMemo, useState } from "react";
import {
  CalendarDays,
  ChevronLeft,
  ChevronRight,
} from "lucide-react";

import api from "../api/api";
import "./VehicleAvailabilityCalendar.css";

const dayNames = [
  "Pon",
  "Uto",
  "Sre",
  "Čet",
  "Pet",
  "Sub",
  "Ned",
];

function getMonthStart(date) {
  return new Date(
    date.getFullYear(),
    date.getMonth(),
    1
  );
}

function getCalendarDays(month) {
  const year = month.getFullYear();
  const monthIndex = month.getMonth();
  const numberOfDays = new Date(
    year,
    monthIndex + 1,
    0
  ).getDate();

  const firstDayIndex =
    (new Date(year, monthIndex, 1).getDay() + 6) % 7;

  const days = [];

  for (let index = 0; index < firstDayIndex; index += 1) {
    days.push(null);
  }

  for (let day = 1; day <= numberOfDays; day += 1) {
    days.push(new Date(year, monthIndex, day));
  }

  return days;
}

function isPastDate(date) {
  const today = new Date();
  today.setHours(0, 0, 0, 0);

  const selectedDate = new Date(date);
  selectedDate.setHours(0, 0, 0, 0);

  return selectedDate < today;
}

function getPeriodForDate(date, unavailablePeriods) {
  const dayStart = new Date(date);
  dayStart.setHours(0, 0, 0, 0);

  const dayEnd = new Date(date);
  dayEnd.setHours(23, 59, 59, 999);

  return unavailablePeriods.find((period) => {
    const periodStart = new Date(
      period.startDateTimeUtc
    );

    const periodEnd = new Date(period.endDateTimeUtc);

    return periodStart <= dayEnd && periodEnd >= dayStart;
  });
}

export default function VehicleAvailabilityCalendar({
  vehicleId,
}) {
  const [visibleMonth, setVisibleMonth] = useState(
    getMonthStart(new Date())
  );

  const [unavailablePeriods, setUnavailablePeriods] =
    useState([]);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const loadCalendar = async () => {
      try {
        setError("");

        const response = await api.get(
          `/vehicles/${vehicleId}/calendar`
        );

        setUnavailablePeriods(response.data);
      } catch (requestError) {
        setError(
          requestError.response?.data?.detail ||
            "Kalendar trenutno nije moguće učitati."
        );
      } finally {
        setLoading(false);
      }
    };

    loadCalendar();
  }, [vehicleId]);

  const calendarDays = useMemo(
    () => getCalendarDays(visibleMonth),
    [visibleMonth]
  );

  const currentMonth = getMonthStart(new Date());

  const isCurrentMonth =
    visibleMonth.getFullYear() ===
      currentMonth.getFullYear() &&
    visibleMonth.getMonth() ===
      currentMonth.getMonth();

  const previousMonth = () => {
    if (isCurrentMonth) {
      return;
    }

    setVisibleMonth(
      new Date(
        visibleMonth.getFullYear(),
        visibleMonth.getMonth() - 1,
        1
      )
    );
  };

  const nextMonth = () => {
    setVisibleMonth(
      new Date(
        visibleMonth.getFullYear(),
        visibleMonth.getMonth() + 1,
        1
      )
    );
  };

  const monthTitle = visibleMonth.toLocaleDateString(
    "sr-Latn-RS",
    {
      month: "long",
      year: "numeric",
    }
  );

  return (
    <section className="availability-calendar">
      <div className="availability-calendar-heading">
        <div>
          <span className="calendar-heading-icon">
            <CalendarDays size={20} />
          </span>

          <div>
            <h2>Dostupnost vozila</h2>
            <p>
              Proverite slobodne datume pre rezervacije.
            </p>
          </div>
        </div>

        <div className="calendar-navigation">
          <button
            type="button"
            disabled={isCurrentMonth}
            onClick={previousMonth}
            aria-label="Prethodni mesec"
          >
            <ChevronLeft size={19} />
          </button>

          <strong>{monthTitle}</strong>

          <button
            type="button"
            onClick={nextMonth}
            aria-label="Sledeći mesec"
          >
            <ChevronRight size={19} />
          </button>
        </div>
      </div>

      {loading && (
        <p className="calendar-status">
          Učitavanje kalendara...
        </p>
      )}

      {error && (
        <p className="calendar-status calendar-error">
          {error}
        </p>
      )}

      {!loading && !error && (
        <>
          <div className="calendar-weekdays">
            {dayNames.map((dayName) => (
              <span key={dayName}>{dayName}</span>
            ))}
          </div>

          <div className="calendar-days">
            {calendarDays.map((date, index) => {
              if (!date) {
                return (
                  <span
                    className="calendar-empty-day"
                    key={`empty-${index}`}
                  />
                );
              }

              const period = getPeriodForDate(
                date,
                unavailablePeriods
              );

              const past = isPastDate(date);

              let className = "calendar-day available";
              let title = "Dostupno";

              if (past) {
                className = "calendar-day past";
                title = "Prošao datum";
              } else if (period?.type === "Reserved") {
                className = "calendar-day reserved";
                title = "Rezervisano";
              } else if (period) {
                className = "calendar-day blocked";
                title = "Nije dostupno";
              }

              return (
                <span
                  className={className}
                  title={title}
                  key={date.toISOString()}
                >
                  {date.getDate()}
                </span>
              );
            })}
          </div>

          <div className="calendar-legend">
            <span>
              <i className="available" />
              Dostupno
            </span>

            <span>
              <i className="unavailable" />
              Nedostupno
            </span>

            <span>
              <i className="past" />
              Prošao datum
            </span>
          </div>
        </>
      )}
    </section>
  );
}