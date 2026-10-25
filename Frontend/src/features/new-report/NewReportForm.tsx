
"use client";

import Link from "next/link";
import { useEffect, useState } from "react";

import {
  analyzeReport,
  createReport,
  getCategories,
  getNeighborhoods,
} from "@/lib/api";

import type {
  AnalyzeResult,
  Category,
  Neighborhood,
  Priority,
  Report,
} from "@/lib/types";

const PRIORITIES: { value: Priority; label: string }[] = [
  { value: "Low", label: "Baja" },
  { value: "Medium", label: "Media" },
  { value: "High", label: "Alta" },
];

const fieldClass =
  "w-full rounded-sm border border-line bg-surface p-3 text-ink outline-none focus:border-accent disabled:opacity-60";

export function NewReportForm() {
  const [description, setDescription] = useState("");
  const [address, setAddress] = useState("");
  const [categoryId, setCategoryId] = useState("");
  const [neighborhoodId, setNeighborhoodId] = useState("");

  const [priority, setPriority] = useState<Priority>("Medium");
  const [summary, setSummary] = useState("");

  const [categories, setCategories] = useState<Category[]>([]);
  const [neighborhoods, setNeighborhoods] = useState<Neighborhood[]>([]);
  const [loadingOptions, setLoadingOptions] = useState(true);
  const [optionsError, setOptionsError] = useState("");

  const [analysis, setAnalysis] = useState<AnalyzeResult | null>(null);
  const [analyzedDescription, setAnalyzedDescription] = useState("");
  const [analyzing, setAnalyzing] = useState(false);
  const [analysisError, setAnalysisError] = useState("");

  const [saving, setSaving] = useState(false);
  const [saveError, setSaveError] = useState("");
  const [createdReport, setCreatedReport] = useState<Report | null>(null);

  const analysisIsCurrent =
    analysis !== null &&
    analyzedDescription === description.trim();

  useEffect(() => {
    let active = true;

    async function loadOptions() {
      try {
        const [categoryData, neighborhoodData] = await Promise.all([
          getCategories(),
          getNeighborhoods(),
        ]);

        if (!active) return;

        setCategories(categoryData);
        setNeighborhoods(neighborhoodData);
        setOptionsError("");
      } catch {
        if (active) {
          setOptionsError(
            "No se pudieron cargar las categorías y los barrios. Verificá la conexión con la API."
          );
        }
      } finally {
        if (active) setLoadingOptions(false);
      }
    }

    void loadOptions();

    return () => {
      active = false;
    };
  }, []);

  async function handleAnalyze() {
    const text = description.trim();

    if (!text) {
      setAnalysisError("Primero describí el problema.");
      return;
    }

    setAnalyzing(true);
    setAnalysisError("");
    setAnalysis(null);
    setAnalyzedDescription("");

    try {
      const result = await analyzeReport(text);

      setAnalysis(result);
      setAnalyzedDescription(text);
      setPriority(result.priority);
      setSummary(result.summary);

      if (
        result.categoryId !== null &&
        categories.some((category) => category.id === result.categoryId)
      ) {
        setCategoryId(String(result.categoryId));
      }
    } catch (error) {
      setAnalysisError(
        error instanceof Error
          ? error.message
          : "No se pudo analizar el reclamo."
      );
    } finally {
      setAnalyzing(false);
    }
  }

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (saving || analyzing || createdReport) return;

    setSaveError("");

    const cleanDescription = description.trim();
    const cleanAddress = address.trim();

    if (!cleanDescription || !cleanAddress) {
      setSaveError("Completá la descripción y la dirección.");
      return;
    }

    const selectedCategory = Number(categoryId);
    const selectedNeighborhood = Number(neighborhoodId);

    if (
      !categories.some((category) => category.id === selectedCategory) ||
      !neighborhoods.some(
        (neighborhood) => neighborhood.id === selectedNeighborhood
      )
    ) {
      setSaveError("Seleccioná una categoría y un barrio válidos.");
      return;
    }

    setSaving(true);

    try {
      const created = await createReport({
        description: cleanDescription,
        address: cleanAddress,
        categoryId: selectedCategory,
        neighborhoodId: selectedNeighborhood,
        priority,
        summary: summary.trim() || null,
      });

      setCreatedReport(created);
    } catch (error) {
      setSaveError(
        error instanceof Error
          ? error.message
          : "No se pudo registrar el reclamo."
      );
    } finally {
      setSaving(false);
    }
  }

  function resetForm() {
    setDescription("");
    setAddress("");
    setCategoryId("");
    setNeighborhoodId("");
    setPriority("Medium");
    setSummary("");
    setAnalysis(null);
    setAnalyzedDescription("");
    setAnalysisError("");
    setSaveError("");
    setCreatedReport(null);
  }

  if (createdReport) {
    return (
      <section className="mx-auto max-w-2xl">
        <div
          role="status"
          className="rounded-md border border-resolved bg-surface p-8"
        >
          <div className="mb-4 inline-flex rounded-sm bg-resolved-soft px-3 py-1 text-sm font-medium text-resolved">
            Registro exitoso
          </div>

          <h1 className="text-3xl font-bold text-ink">
            Reclamo registrado correctamente
          </h1>

          <p className="mt-3 text-muted">
            El reclamo se guardó en el sistema y ya puede
            consultarse desde el panel de gestión.
          </p>

          <div className="mt-6 rounded-sm bg-canvas p-5">
            <p className="text-sm text-muted">
              Número de reclamo
            </p>

            <p className="mt-1 text-4xl font-bold text-accent-strong">
              #{createdReport.id}
            </p>

            <p className="mt-4 text-sm text-muted">
              Estado inicial: Pendiente
            </p>
          </div>

          <div className="mt-6 flex flex-wrap gap-3">
            <Link
              href="/"
              className="rounded-sm bg-accent px-5 py-3 font-medium text-white hover:bg-accent-strong"
            >
              Ir al panel de reclamos
            </Link>

            <button
              type="button"
              onClick={resetForm}
              className="rounded-sm border border-line px-5 py-3 font-medium hover:bg-canvas"
            >
              Crear otro reclamo
            </button>
          </div>
        </div>
      </section>
    );
  }

  return (
    <section className="space-y-6">
      <header>
        <p className="text-sm font-medium text-accent">
          REGISTRO CIUDADANO
        </p>

        <h1 className="mt-2 text-3xl font-bold text-ink">
          Nuevo reclamo
        </h1>

        <p className="mt-2 max-w-2xl text-muted">
          Contanos qué problema encontraste en tu barrio.
          Podés utilizar inteligencia artificial para recibir
          sugerencias antes de registrar el reclamo.
        </p>
      </header>

      <form onSubmit={handleSubmit}>
        <div className="grid items-start gap-6 lg:grid-cols-3">
          <div className="space-y-5 lg:col-span-2">
            <div className="rounded-md border border-line bg-surface p-6">
              <h2 className="text-xl font-semibold">
                Información del problema
              </h2>

              <p className="mt-1 text-sm text-muted">
                Completá los datos de la incidencia urbana.
              </p>

              <div className="mt-6 space-y-5">
                <div>
                  <label
                    htmlFor="description"
                    className="mb-2 block font-medium"
                  >
                    Descripción del problema *
                  </label>

                  <textarea
                    id="description"
                    required
                    rows={5}
                    maxLength={2000}
                    value={description}
                    onChange={(event) => {
                      setDescription(event.target.value);
                      setAnalysisError("");
                    }}
                    placeholder="Ej.: Hay un bache de gran tamaño que dificulta la circulación..."
                    className={fieldClass}
                  />

                  <div className="mt-1 flex justify-between gap-2 text-xs text-muted">
                    <span>
                      Describí el problema con claridad.
                    </span>
                    <span>{description.length}/2000</span>
                  </div>
                </div>

                <div>
                  <label
                    htmlFor="address"
                    className="mb-2 block font-medium"
                  >
                    Dirección *
                  </label>

                  <input
                    id="address"
                    required
                    maxLength={200}
                    value={address}
                    onChange={(event) => setAddress(event.target.value)}
                    placeholder="Ej.: Av. Colón 1500"
                    className={fieldClass}
                  />
                </div>
              </div>
            </div>

            <div className="rounded-md border border-line bg-surface p-6">
              <h2 className="text-xl font-semibold">
                Clasificación del reclamo
              </h2>

              <p className="mt-1 text-sm text-muted">
                Revisá la categoría, el barrio y la prioridad.
                Las sugerencias de IA pueden modificarse.
              </p>

              <div className="mt-5 grid gap-4 sm:grid-cols-2">
                <div>
                  <label
                    htmlFor="category"
                    className="mb-2 block font-medium"
                  >
                    Categoría *
                  </label>

                  <select
                    id="category"
                    required
                    value={categoryId}
                    onChange={(event) => setCategoryId(event.target.value)}
                    disabled={loadingOptions || Boolean(optionsError)}
                    className={fieldClass}
                  >
                    <option value="">
                      Seleccionar categoría
                    </option>

                    {categories.map((category) => (
                      <option key={category.id} value={category.id}>
                        {category.name}
                      </option>
                    ))}
                  </select>
                </div>

                <div>
                  <label
                    htmlFor="neighborhood"
                    className="mb-2 block font-medium"
                  >
                    Barrio *
                  </label>

                  <select
                    id="neighborhood"
                    required
                    value={neighborhoodId}
                    onChange={(event) => setNeighborhoodId(event.target.value)}
                    disabled={loadingOptions || Boolean(optionsError)}
                    className={fieldClass}
                  >
                    <option value="">
                      Seleccionar barrio
                    </option>

                    {neighborhoods.map((neighborhood) => (
                      <option
                        key={neighborhood.id}
                        value={neighborhood.id}
                      >
                        {neighborhood.name}
                      </option>
                    ))}
                  </select>
                </div>
              </div>

              <div className="mt-5">
                <label
                  htmlFor="priority"
                  className="mb-2 block font-medium"
                >
                  Prioridad
                </label>

                <select
                  id="priority"
                  value={priority}
                  onChange={(event) =>
                    setPriority(event.target.value as Priority)
                  }
                  className={fieldClass}
                >
                  {PRIORITIES.map((item) => (
                    <option key={item.value} value={item.value}>
                      {item.label}
                    </option>
                  ))}
                </select>

                <p className="mt-1 text-xs text-muted">
                  Podés modificar la prioridad que sugiera la IA.
                </p>
              </div>

              {loadingOptions && (
                <p role="status" className="mt-4 text-sm text-muted">
                  Cargando categorías y barrios...
                </p>
              )}

              {optionsError && (
                <p role="alert" className="mt-4 text-sm text-high">
                  {optionsError}
                </p>
              )}
            </div>

            <div className="rounded-md border border-line bg-surface p-6">
              <h2 className="text-xl font-semibold">
                Resumen del reclamo
              </h2>

              <p className="mt-1 text-sm text-muted">
                Podés utilizar el resumen de Gemini,
                editarlo o dejarlo vacío.
              </p>

              <textarea
                id="summary"
                aria-label="Resumen del reclamo"
                rows={3}
                maxLength={200}
                value={summary}
                onChange={(event) => setSummary(event.target.value)}
                placeholder="Resumen breve del problema..."
                className={`mt-4 ${fieldClass}`}
              />

              <p className="mt-1 text-right text-xs text-muted">
                {summary.length}/200
              </p>
            </div>

            {saveError && (
              <div
                role="alert"
                className="rounded-sm border border-high bg-high-soft p-4 text-sm text-high"
              >
                {saveError}
              </div>
            )}

            <div className="flex flex-wrap items-center justify-between gap-3">
              <p className="text-sm text-muted">
                * Campos obligatorios
              </p>

              <button
                type="submit"
                disabled={
                  saving ||
                  analyzing ||
                  loadingOptions ||
                  Boolean(optionsError)
                }
                className="rounded-sm bg-accent px-6 py-3 font-semibold text-white hover:bg-accent-strong disabled:cursor-not-allowed disabled:opacity-50"
              >
                {saving
                  ? "Registrando reclamo..."
                  : "Registrar reclamo"}
              </button>
            </div>
          </div>

          <aside className="h-fit rounded-md border border-line bg-surface p-6">
            <div className="mb-4 inline-flex rounded-sm bg-accent-soft px-3 py-1 text-sm font-medium text-accent-strong">
              Asistencia inteligente
            </div>

            <h2 className="text-xl font-semibold">
              Analizá tu reclamo con IA
            </h2>

            <p className="mt-3 text-sm text-muted">
              Gemini analiza la descripción y propone
              una categoría, prioridad y resumen.
            </p>

            <div className="mt-6 space-y-3">
              <div className="rounded-sm bg-canvas p-3 text-sm">
                <p className="text-xs text-muted">
                  Categoría sugerida
                </p>

                <p className="mt-1 font-medium">
                  {analysisIsCurrent
                    ? categories.find(
                        (category) =>
                          category.id === analysis.categoryId
                      )?.name ?? "Sin categoría identificada"
                    : "Pendiente de análisis"}
                </p>
              </div>

              <div className="rounded-sm bg-canvas p-3 text-sm">
                <p className="text-xs text-muted">
                  Prioridad sugerida
                </p>

                <p className="mt-1 font-medium">
                  {analysisIsCurrent
                    ? PRIORITIES.find(
                        (item) =>
                          item.value === analysis.priority
                      )?.label
                    : "Pendiente de análisis"}
                </p>
              </div>

              <div className="rounded-sm bg-canvas p-3 text-sm">
                <p className="text-xs text-muted">
                  Resumen sugerido
                </p>

                <p className="mt-1 whitespace-pre-wrap font-medium">
                  {analysisIsCurrent
                    ? analysis.summary
                    : "Pendiente de análisis"}
                </p>
              </div>
            </div>

            {analysisIsCurrent && analysis.usedFallback && (
              <p
                role="status"
                className="mt-4 rounded-sm border border-pending bg-pending-soft p-3 text-sm text-pending"
              >
                No se pudo utilizar Gemini.
                Se aplicó un análisis alternativo.
              </p>
            )}

            {analysis !== null && !analysisIsCurrent && (
              <p role="status" className="mt-4 text-sm text-pending">
                Modificaste la descripción. Volvé a analizar
                para actualizar las sugerencias.
              </p>
            )}

            <div className="mt-5 rounded-sm border border-line p-3">
              <p className="text-xs text-muted">
                La inteligencia artificial brinda sugerencias.
                Revisá los datos antes de registrar el reclamo.
              </p>
            </div>

            <button
              type="button"
              onClick={handleAnalyze}
              disabled={
                analyzing ||
                saving ||
                !description.trim() ||
                loadingOptions ||
                Boolean(optionsError)
              }
              className="mt-5 w-full rounded-sm bg-accent px-4 py-3 font-medium text-white hover:bg-accent-strong disabled:cursor-not-allowed disabled:opacity-50"
            >
              {analyzing
                ? "Analizando reclamo..."
                : analysisIsCurrent
                  ? "Volver a analizar"
                  : "Analizar con IA"}
            </button>

            {analysisError && (
              <p role="alert" className="mt-3 text-sm text-high">
                {analysisError}
              </p>
            )}

            {analysisIsCurrent && !analysis.usedFallback && (
              <p
                role="status"
                className="mt-3 text-center text-sm text-resolved"
              >
                Análisis completado correctamente.
              </p>
            )}
          </aside>
        </div>
      </form>
    </section>
  );
}