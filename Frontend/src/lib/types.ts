
 // Tipos que reflejan los DTOs de la API (ReclamosUrbanos.Presentation/DTOs).
 // La API serializa en camelCase y los enums como texto.

export type Status = "Pending" | "InProgress" | "Resolved";
export type Priority = "Low" | "Medium" | "High";

export const STATUSES: Status[] = ["Pending", "InProgress", "Resolved"];
export const PRIORITIES: Priority[] = ["Low", "Medium", "High"];

export interface Category {
  id: number;
  name: string;
  description: string | null;
}

export interface Neighborhood {
  id: number;
  name: string;
}

export interface Report {
  id: number;
  description: string;
  address: string;
  /** Resumen generado por la IA (puede no existir). */
  summary: string | null;
  categoryId: number;
  neighborhoodId: number;
  status: Status;
  priority: Priority;
  /** Fecha en formato ISO 8601 (UTC). */
  createdAt: string;
}

/** Filtros de GET /api/reports. Todos son opcionales. */
export interface ReportFilters {
  categoryId?: number;
  neighborhoodId?: number;
  status?: Status;
  priority?: Priority;
}

/**
 * Cuerpo de POST /api/reports.
 * priority y summary son los valores que el usuario confirmó de la sugerencia de la IA.
 * Confirmar con CreateReportInput.cs que los nombres coinciden.
 */
export interface CreateReportInput {
  description: string;
  address: string;
  categoryId: number;
  neighborhoodId: number;
  priority?: Priority;
  summary?: string | null;
}

/** Cuerpo de PUT /api/reports/{id}. */
export type UpdateReportInput = CreateReportInput;

/** Respuesta de POST /api/reports/analyze: es una sugerencia, no guarda nada. */
export interface AnalyzeResult {
  /** null si la IA no pudo determinar la categoría: el usuario debe elegirla. */
  categoryId: number | null;

  /** Prioridad sugerida por la inteligencia artificial. */
  priority: Priority;

  /** Resumen generado por la inteligencia artificial. */
  summary: string;

  /** true si la IA falló y se devolvió el plan B. */
  usedFallback: boolean;

  /** true si la IA detectó una posible emergencia o delito. */
  isEmergency: boolean;
}

export interface CategoryInput {
  name: string;
  description?: string | null;
}

export interface NeighborhoodInput {
  name: string;
}
