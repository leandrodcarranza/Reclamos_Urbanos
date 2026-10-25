
"use client";

import { useEffect, useMemo, useState } from "react";
import {
  changeReportStatus,
  getCategories,
  getNeighborhoods,
  getReports,
} from "../../lib/api";
import type {
  Category,
  Neighborhood,
  Priority,
  Report,
  Status,
} from "../../lib/types";

const statusLabels: Record<Status, string> = {
  Pending: "Pendiente",
  InProgress: "En proceso",
  Resolved: "Resuelto",
};

const priorityLabels: Record<Priority, string> = {
  Low: "Baja",
  Medium: "Media",
  High: "Alta",
};

const fieldStyle = {
  backgroundColor: "#0b1120",
  color: "#f8fafc",
  border: "1px solid #334155",
  borderRadius: 8,
  padding: 11,
  minWidth: 140,
};

export default function Dashboard() {
  const [reports, setReports] = useState<Report[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [neighborhoods, setNeighborhoods] = useState<Neighborhood[]>([]);

  const [search, setSearch] = useState("");
  const [categoryFilter, setCategoryFilter] = useState("");
  const [neighborhoodFilter, setNeighborhoodFilter] = useState("");
  const [statusFilter, setStatusFilter] = useState("");
  const [priorityFilter, setPriorityFilter] = useState("");

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [savingId, setSavingId] = useState<number | null>(null);

  useEffect(() => {
    async function loadData() {
      try {
        const [reportData, categoryData, neighborhoodData] =
          await Promise.all([
            getReports(),
            getCategories(),
            getNeighborhoods(),
          ]);

        setReports(reportData);
        setCategories(categoryData);
        setNeighborhoods(neighborhoodData);
      } catch (err) {
        setError(
          err instanceof Error
            ? err.message
            : "No se pudieron cargar los reclamos."
        );
      } finally {
        setLoading(false);
      }
    }

    void loadData();
  }, []);

  const categoryNames = useMemo(
    () => new Map(categories.map((item) => [item.id, item.name])),
    [categories]
  );

  const neighborhoodNames = useMemo(
    () => new Map(neighborhoods.map((item) => [item.id, item.name])),
    [neighborhoods]
  );

  const filteredReports = useMemo(() => {
    const query = search.toLowerCase().trim();

    return reports.filter((report) => {
      const categoryName =
        categoryNames.get(report.categoryId) ?? "";

      const neighborhoodName =
        neighborhoodNames.get(report.neighborhoodId) ?? "";

      const searchableText = [
        report.id,
        report.description,
        report.address,
        report.summary,
        categoryName,
        neighborhoodName,
      ]
        .join(" ")
        .toLowerCase();

      return (
        searchableText.includes(query) &&
        (!categoryFilter ||
          String(report.categoryId) === categoryFilter) &&
        (!neighborhoodFilter ||
          String(report.neighborhoodId) === neighborhoodFilter) &&
        (!statusFilter || report.status === statusFilter) &&
        (!priorityFilter || report.priority === priorityFilter)
      );
    });
  }, [
    reports,
    search,
    categoryFilter,
    neighborhoodFilter,
    statusFilter,
    priorityFilter,
    categoryNames,
    neighborhoodNames,
  ]);

  const pending = reports.filter(
    (report) => report.status === "Pending"
  ).length;

  const inProgress = reports.filter(
    (report) => report.status === "InProgress"
  ).length;

  const resolved = reports.filter(
    (report) => report.status === "Resolved"
  ).length;

  async function handleStatusChange(id: number, status: Status) {
    setSavingId(id);
    setError("");

    try {
      await changeReportStatus(id, status);

      setReports((current) =>
        current.map((report) =>
          report.id === id ? { ...report, status } : report
        )
      );
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : "No se pudo guardar el estado."
      );
    } finally {
      setSavingId(null);
    }
  }

  return (
    <div
      style={{
        backgroundColor: "#0b1120",
        color: "#f8fafc",
        padding: 28,
        borderRadius: 16,
        minHeight: "80vh",
      }}
    >
      <h1 style={{ fontSize: 28, fontWeight: "bold" }}>
        Panel de reclamos
      </h1>

      <p style={{ color: "#94a3b8", marginTop: 8 }}>
        Monitoreo y gestión de incidencias urbanas
      </p>

      <div
        style={{
          display: "grid",
          gridTemplateColumns:
            "repeat(auto-fit, minmax(180px, 1fr))",
          gap: 18,
          marginTop: 30,
        }}
      >
        {[
          { label: "Total de reclamos", value: reports.length },
          { label: "Pendientes", value: pending },
          { label: "En proceso", value: inProgress },
          { label: "Resueltos", value: resolved },
        ].map((item) => (
          <div
            key={item.label}
            style={{
              backgroundColor: "#151f32",
              border: "1px solid #263449",
              borderRadius: 14,
              padding: 24,
            }}
          >
            <p style={{ color: "#94a3b8", fontSize: 14 }}>
              {item.label}
            </p>

            <p
              style={{
                fontSize: 34,
                fontWeight: "bold",
                marginTop: 12,
              }}
            >
              {item.value}
            </p>
          </div>
        ))}
      </div>

      <section
        style={{
          backgroundColor: "#151f32",
          border: "1px solid #263449",
          borderRadius: 14,
          padding: 24,
          marginTop: 30,
        }}
      >
        <h2 style={{ fontSize: 22, fontWeight: "bold" }}>
          Gestión de reclamos
        </h2>

        <p style={{ color: "#94a3b8", marginTop: 8 }}>
          Consultá y administrá los reclamos registrados.
        </p>

        <div
          style={{
            display: "flex",
            flexWrap: "wrap",
            gap: 12,
            marginTop: 24,
            marginBottom: 24,
          }}
        >
          <input
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="Buscar reclamo..."
            aria-label="Buscar reclamo"
            style={{ ...fieldStyle, flex: "2 1 180px" }}
          />

          <select
            value={categoryFilter}
            onChange={(event) =>
              setCategoryFilter(event.target.value)
            }
            aria-label="Filtrar por categoría"
            style={fieldStyle}
          >
            <option value="">Todas las categorías</option>
            {categories.map((category) => (
              <option key={category.id} value={category.id}>
                {category.name}
              </option>
            ))}
          </select>

          <select
            value={neighborhoodFilter}
            onChange={(event) =>
              setNeighborhoodFilter(event.target.value)
            }
            aria-label="Filtrar por barrio"
            style={fieldStyle}
          >
            <option value="">Todos los barrios</option>
            {neighborhoods.map((neighborhood) => (
              <option key={neighborhood.id} value={neighborhood.id}>
                {neighborhood.name}
              </option>
            ))}
          </select>

          <select
            value={statusFilter}
            onChange={(event) =>
              setStatusFilter(event.target.value)
            }
            aria-label="Filtrar por estado"
            style={fieldStyle}
          >
            <option value="">Todos los estados</option>
            <option value="Pending">Pendiente</option>
            <option value="InProgress">En proceso</option>
            <option value="Resolved">Resuelto</option>
          </select>

          <select
            value={priorityFilter}
            onChange={(event) =>
              setPriorityFilter(event.target.value)
            }
            aria-label="Filtrar por prioridad"
            style={fieldStyle}
          >
            <option value="">Todas las prioridades</option>
            <option value="High">Alta</option>
            <option value="Medium">Media</option>
            <option value="Low">Baja</option>
          </select>
        </div>

        {error && (
          <p
            role="alert"
            style={{
              color: "#fca5a5",
              marginBottom: 18,
            }}
          >
            {error}
          </p>
        )}

        {loading ? (
          <p style={{ color: "#94a3b8" }}>
            Cargando reclamos...
          </p>
        ) : (
          <div style={{ overflowX: "auto" }}>
            <table
              style={{
                width: "100%",
                borderCollapse: "collapse",
                textAlign: "left",
              }}
            >
              <thead>
                <tr>
                  {[
                    "ID",
                    "Descripción",
                    "Categoría",
                    "Barrio",
                    "Prioridad",
                    "Estado",
                  ].map((column) => (
                    <th
                      key={column}
                      style={{
                        color: "#94a3b8",
                        fontSize: 13,
                        padding: 14,
                        borderBottom: "1px solid #334155",
                      }}
                    >
                      {column}
                    </th>
                  ))}
                </tr>
              </thead>

              <tbody>
                {filteredReports.map((report) => (
                  <tr key={report.id}>
                    <td style={{ padding: 14 }}>
                      #{report.id}
                    </td>

                    <td style={{ padding: 14 }}>
                      {report.description}
                    </td>

                    <td style={{ padding: 14 }}>
                      {categoryNames.get(report.categoryId) ??
                        "Sin categoría"}
                    </td>

                    <td style={{ padding: 14 }}>
                      {neighborhoodNames.get(
                        report.neighborhoodId
                      ) ?? "Sin barrio"}
                    </td>

                    <td
                      style={{
                        padding: 14,
                        color:
                          report.priority === "High"
                            ? "#f87171"
                            : report.priority === "Medium"
                              ? "#fbbf24"
                              : "#4ade80",
                        fontWeight: "bold",
                      }}
                    >
                      {priorityLabels[report.priority]}
                    </td>

                    <td style={{ padding: 14 }}>
                      <select
                        value={report.status}
                        disabled={savingId === report.id}
                        onChange={(event) =>
                          void handleStatusChange(
                            report.id,
                            event.target.value as Status
                          )
                        }
                        aria-label={`Estado del reclamo ${report.id}`}
                        style={{
                          ...fieldStyle,
                          backgroundColor: "#1e3a5f",
                        }}
                      >
                        <option value="Pending">
                          Pendiente
                        </option>
                        <option value="InProgress">
                          En proceso
                        </option>
                        <option value="Resolved">
                          Resuelto
                        </option>
                      </select>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>

            {filteredReports.length === 0 && (
              <p
                style={{
                  color: "#94a3b8",
                  textAlign: "center",
                  padding: 30,
                }}
              >
                No se encontraron reclamos con esos filtros.
              </p>
            )}
          </div>
        )}
      </section>
    </div>
  );
}
