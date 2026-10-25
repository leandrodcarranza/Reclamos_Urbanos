import type { Priority, Status } from "./types";

// Textos para mostrar en pantalla (la API usa los valores en inglés).

export const statusLabels: Record<Status, string> = {
  Pending: "Pendiente",
  InProgress: "En proceso",
  Resolved: "Resuelto",
};

export const priorityLabels: Record<Priority, string> = {
  Low: "Baja",
  Medium: "Media",
  High: "Alta",
};

/** Fecha corta en español de Argentina, por ejemplo "24/10/2026 18:55". */
export function formatDate(iso: string): string {
  return new Intl.DateTimeFormat("es-AR", {
    dateStyle: "short",
    timeStyle: "short",
  }).format(new Date(iso));
}
