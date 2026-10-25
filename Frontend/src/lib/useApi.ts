"use client";

import { useCallback, useEffect, useState } from "react";

interface ApiState<T> {
  data: T | null;
  error: string | null;
  loading: boolean;
}

/**
 * Carga datos de la API desde un componente de cliente.
 *
 * const { data, error, loading, reload } = useApi(() => getReports(filters), [filters]);
 *
 * `deps` indica cuándo hay que volver a pedir los datos (por ejemplo, al cambiar un filtro).
 * `reload()` fuerza una nueva carga, útil después de cambiar el estado de un reclamo.
 */
export function useApi<T>(fetcher: () => Promise<T>, deps: readonly unknown[] = []) {
  const [state, setState] = useState<ApiState<T>>({
    data: null,
    error: null,
    loading: true,
  });
  const [version, setVersion] = useState(0);

  useEffect(() => {
    let cancelled = false;

    fetcher()
      .then((data) => {
        if (!cancelled) setState({ data, error: null, loading: false });
      })
      .catch((e: unknown) => {
        if (cancelled) return;
        const error = e instanceof Error ? e.message : "Ocurrió un error inesperado.";
        setState((prev) => ({ data: prev.data, error, loading: false }));
      });

    return () => {
      cancelled = true;
    };
    // fetcher cambia en cada render; el control de recargas lo hacen deps y version.
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [...deps, version]);

  const reload = useCallback(() => setVersion((v) => v + 1), []);

  return { ...state, reload };
}
