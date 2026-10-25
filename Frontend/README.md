# Reclamos Urbanos — Frontend

Interfaz web del sistema de reclamos urbanos. Se conecta a la API de .NET del mismo repositorio (`Backend/`).
Hecho con Next.js 16 (App Router), React 19, TypeScript y Tailwind CSS 4.

## Cómo correrlo

Requisitos: Node.js 20.9 o superior (recomendado 22) y la API en ejecución.

1. **Levantar la API** desde Visual Studio con el perfil **`http`** (queda en `http://localhost:5121`).
2. Desde la carpeta `Frontend/`:

   ```powershell
   copy .env.example .env.local
   npm install
   npm run dev
   ```

3. Abrir <http://localhost:3000>. En el encabezado tiene que decir **"API conectada"**.

Otros comandos: `npm run build` (compila), `npm run lint` (revisa el código).

## Estructura y responsables

```
src/
├── app/                    Rutas y estilos globales
│   ├── layout.tsx          Estructura común (encabezado + contenido)
│   ├── globals.css         Colores, tipografía y reglas globales
│   ├── page.tsx            Panel de reclamos          ← Franco
│   └── nuevo/page.tsx      Nuevo reclamo              ← Mauricio
├── components/             Piezas compartidas (encabezado, indicador de la API, badges)
├── features/
│   ├── dashboard/          Componentes del Panel      ← Franco
│   └── new-report/         Componentes del formulario ← Mauricio
└── lib/                    Compartido
    ├── api.ts              Todas las llamadas a la API
    ├── types.ts            Tipos de los DTOs
    ├── labels.ts           Textos en español para estados y prioridades
    └── useApi.ts           Hook para cargar datos
```

### Cómo no pisarse

- Cada uno trabaja en **su pantalla** (`app/page.tsx` o `app/nuevo/page.tsx`) y en **su carpeta de `features/`**.
- `lib/`, `components/`, `layout.tsx` y `globals.css` son compartidos: si hay que tocarlos, que el cambio sea chico y se avise al grupo.
- Ramas: `feature/frontend-dashboard` y `feature/frontend-new-report`. Todo entra por pull request con una aprobación.
- No subir `.env.local` ni `node_modules` (ya están ignorados).

## Cómo llamar a la API

Todas las funciones están en `src/lib/api.ts`. Los errores llegan como `ApiError` y su `message` ya está listo para mostrarle al usuario.

Cargar datos en un componente (los componentes que usan hooks llevan `"use client"` arriba):

```tsx
"use client";

import { useApi } from "@/lib/useApi";
import { getReports } from "@/lib/api";

export function ReportList({ status }: { status?: Status }) {
  const { data, error, loading, reload } = useApi(() => getReports({ status }), [status]);

  if (loading) return <p>Cargando…</p>;
  if (error) return <p role="alert">{error}</p>;
  return <ul>{data?.map((r) => <li key={r.id}>{r.description}</li>)}</ul>;
}
```

Cambiar el estado de un reclamo y volver a cargar el listado:

```tsx
await changeReportStatus(report.id, "InProgress");
reload();
```

Flujo del formulario con IA:

```tsx
const suggestion = await analyzeReport(description);   // no guarda nada
// Mostrar la sugerencia. Si suggestion.categoryId es null, el usuario elige la categoría.
await createReport({
  description,
  address,
  neighborhoodId,
  categoryId: confirmedCategoryId,
  priority: confirmedPriority,
  summary: suggestion.summary,
});
```

## Reglas de la API a tener en cuenta

- `status` y `priority` viajan **como texto**: `Pending`, `InProgress`, `Resolved` y `Low`, `Medium`, `High`. Para mostrarlos en español, usar `statusLabels` y `priorityLabels` (`lib/labels.ts`) o los componentes `StatusBadge` y `PriorityBadge`.
- Filtros de `getReports`: `categoryId`, `neighborhoodId`, `status` y `priority`. El buscador por texto no existe en la API: se hace en el front sobre la lista cargada.
- `analyzeReport` es una **sugerencia**. Llamarlo al presionar un botón, no en cada tecla: el plan gratuito de Gemini tiene límite de solicitudes por minuto. Si `usedFallback` es `true`, la IA falló y hay que dejar que el usuario complete los datos.
- Borrar una categoría o un barrio con reclamos asociados devuelve `409`.
- `createReport` y `updateReport` aceptan `priority` y `summary`. **Confirmar los nombres exactos con `CreateReportInput.cs`** y ajustar `src/lib/types.ts` si difieren.

## Diseño

- Los colores están definidos en `src/app/globals.css` y se usan como clases de Tailwind: `bg-canvas`, `bg-surface`, `text-ink`, `text-muted`, `border-line`, `bg-accent`, etc.
- Los colores de **estado** (`pending`, `progress`, `resolved`) y de **prioridad** (`high`, `low`) tienen significado: usarlos solo para eso.
- Tipografía: Atkinson Hyperlegible Next, que se instala con el proyecto y funciona sin conexión a internet.
- Dejar siempre el foco visible, etiquetas en los campos de formulario y mensajes de error que expliquen qué hacer.

## Si algo no funciona

| Síntoma | Causa probable |
|---|---|
| El encabezado dice "Sin conexión con la API" | La API no está en ejecución, el perfil no es `http`, o la URL de `.env.local` es incorrecta |
| Error de CORS en la consola del navegador | CORS permite solo `http://localhost:3000` y `http://127.0.0.1:3000`. Si el front corre en otro puerto, agregarlo en `WithOrigins` de `Program.cs` |
| La API redirige o el navegador rechaza el certificado | Usar el perfil `http` y `NEXT_PUBLIC_API_URL=http://localhost:5121` |
| Cambié `.env.local` y no se nota | Reiniciar `npm run dev`: las variables se leen al arrancar |

Este proyecto usa Next.js 16, que tiene cambios respecto a versiones anteriores. Ante cualquier duda, la documentación oficial viene incluida en `node_modules/next/dist/docs/` (después de `npm install`).
