"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { ApiStatus } from "./ApiStatus";

const links = [
  { href: "/", label: "Panel" },
  { href: "/nuevo", label: "Nuevo reclamo" },
];

export function AppHeader() {
  const pathname = usePathname();

  return (
    <header className="border-b border-line bg-surface">
      <div className="mx-auto flex max-w-6xl flex-wrap items-center gap-x-8 gap-y-2 px-6 py-3">
        <Link href="/" className="text-lg font-bold text-accent-strong">
          Reclamos urbanos
        </Link>

        <nav aria-label="Principal" className="flex gap-1">
          {links.map(({ href, label }) => {
            const active = pathname === href;
            return (
              <Link
                key={href}
                href={href}
                aria-current={active ? "page" : undefined}
                className={`rounded-sm px-3 py-1.5 text-base ${
                  active
                    ? "bg-accent-soft font-medium text-accent-strong"
                    : "text-muted hover:text-ink"
                }`}
              >
                {label}
              </Link>
            );
          })}
        </nav>

        <div className="ml-auto">
          <ApiStatus />
        </div>
      </div>
    </header>
  );
}
