"use client";

import { useEffect, useState } from "react";
import { API_URL, getNeighborhoods } from "@/lib/api";

type ConnectionState = "checking" | "ok" | "down";

const labels: Record<ConnectionState, string> = {
  checking: "Conectando con la API",
  ok: "API conectada",
  down: "Sin conexión con la API",
};

const dotStyles: Record<ConnectionState, string> = {
  checking: "bg-low",
  ok: "bg-resolved",
  down: "bg-high",
};

/** Indicador del encabezado: sirve para detectar rápido problemas de URL o de CORS. */
export function ApiStatus() {
  const [state, setState] = useState<ConnectionState>("checking");

  useEffect(() => {
    let cancelled = false;

    getNeighborhoods()
      .then(() => {
        if (!cancelled) setState("ok");
      })
      .catch(() => {
        if (!cancelled) setState("down");
      });

    return () => {
      cancelled = true;
    };
  }, []);

  return (
    <p role="status" title={API_URL} className="flex items-center gap-2 text-sm text-muted">
      <span aria-hidden className={`size-2 rounded-full ${dotStyles[state]}`} />
      {labels[state]}
    </p>
  );
}
