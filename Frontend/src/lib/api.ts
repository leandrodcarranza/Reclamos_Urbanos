import type {
  AnalyzeResult,
  Category,
  CategoryInput,
  CreateReportInput,
  Neighborhood,
  NeighborhoodInput,
  Report,
  ReportFilters,
  Status,
  UpdateReportInput,
} from "./types";

/** URL base de la API. Se configura en .env.local (ver .env.example). */
export const API_URL = (
  process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5121"
).replace(/\/$/, "");

/** Error de la API. `status` es el código HTTP (0 si no hubo conexión). */
export class ApiError extends Error {
  readonly status: number;

  constructor(status: number, message: string) {
    super(message);
    this.name = "ApiError";
    this.status = status;
  }
}

/** La API devuelve los errores como texto plano o como ProblemDetails (JSON). */
async function readError(res: Response): Promise<string> {
  const text = await res.text();
  if (!text) return `La API respondió con el error ${res.status}.`;

  try {
    const data = JSON.parse(text) as {
      title?: string;
      errors?: Record<string, string[]>;
    };
    const firstError = data.errors ? Object.values(data.errors).flat()[0] : undefined;
    return firstError ?? data.title ?? text;
  } catch {
    return text;
  }
}

async function request<T>(path: string, init: RequestInit = {}): Promise<T> {
  let res: Response;
  try {
    res = await fetch(`${API_URL}${path}`, {
      ...init,
      // Solo se envía Content-Type cuando hay cuerpo, para evitar preflights innecesarios.
      headers: init.body ? { "Content-Type": "application/json" } : undefined,
      cache: "no-store",
    });
  } catch {
    throw new ApiError(
      0,
      "No se pudo conectar con la API. Verificá que esté en ejecución."
    );
  }

  if (!res.ok) throw new ApiError(res.status, await readError(res));
  if (res.status === 204) return undefined as T;
  return (await res.json()) as T;
}

function toQuery(filters: ReportFilters): string {
  const params = new URLSearchParams();
  for (const [key, value] of Object.entries(filters)) {
    if (value !== undefined && value !== null && value !== "") {
      params.set(key, String(value));
    }
  }
  const query = params.toString();
  return query ? `?${query}` : "";
}

// ---------- Reclamos ----------

export const getReports = (filters: ReportFilters = {}) =>
  request<Report[]>(`/api/reports${toQuery(filters)}`);

export const getReport = (id: number) => request<Report>(`/api/reports/${id}`);

export const createReport = (input: CreateReportInput) =>
  request<Report>("/api/reports", { method: "POST", body: JSON.stringify(input) });

export const updateReport = (id: number, input: UpdateReportInput) =>
  request<void>(`/api/reports/${id}`, { method: "PUT", body: JSON.stringify(input) });

export const deleteReport = (id: number) =>
  request<void>(`/api/reports/${id}`, { method: "DELETE" });

/** El cuerpo del PATCH es el valor del enum como texto JSON, por ejemplo "InProgress". */
export const changeReportStatus = (id: number, status: Status) =>
  request<void>(`/api/reports/${id}/status`, {
    method: "PATCH",
    body: JSON.stringify(status),
  });

/** Pide a la IA una sugerencia de categoría, prioridad y resumen. No guarda nada. */
export const analyzeReport = (description: string) =>
  request<AnalyzeResult>("/api/reports/analyze", {
    method: "POST",
    body: JSON.stringify({ description }),
  });

// ---------- Categorías ----------

export const getCategories = () => request<Category[]>("/api/categories");

export const createCategory = (input: CategoryInput) =>
  request<Category>("/api/categories", { method: "POST", body: JSON.stringify(input) });

export const updateCategory = (id: number, input: CategoryInput) =>
  request<void>(`/api/categories/${id}`, { method: "PUT", body: JSON.stringify(input) });

/** Devuelve 409 si la categoría tiene reclamos asociados. */
export const deleteCategory = (id: number) =>
  request<void>(`/api/categories/${id}`, { method: "DELETE" });

// ---------- Barrios ----------

export const getNeighborhoods = () => request<Neighborhood[]>("/api/neighborhoods");

export const createNeighborhood = (input: NeighborhoodInput) =>
  request<Neighborhood>("/api/neighborhoods", {
    method: "POST",
    body: JSON.stringify(input),
  });

export const updateNeighborhood = (id: number, input: NeighborhoodInput) =>
  request<void>(`/api/neighborhoods/${id}`, {
    method: "PUT",
    body: JSON.stringify(input),
  });

/** Devuelve 409 si el barrio tiene reclamos asociados. */
export const deleteNeighborhood = (id: number) =>
  request<void>(`/api/neighborhoods/${id}`, { method: "DELETE" });
