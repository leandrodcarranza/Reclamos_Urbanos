import type { Metadata } from "next";
import "@fontsource-variable/atkinson-hyperlegible-next";
import "./globals.css";
import { AppHeader } from "@/components/AppHeader";

export const metadata: Metadata = {
  title: "Reclamos urbanos",
  description: "Registro y seguimiento de reclamos urbanos con ayuda de inteligencia artificial.",
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="es" className="h-full antialiased">
      <body className="flex min-h-full flex-col">
        <AppHeader />
        <main className="mx-auto w-full max-w-6xl flex-1 px-6 py-8">{children}</main>
      </body>
    </html>
  );
}
