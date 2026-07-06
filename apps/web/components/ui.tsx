import Link from "next/link";
import type { ReactNode } from "react";

type ButtonProps = {
  href?: string;
  children: ReactNode;
  variant?: "primary" | "secondary" | "quiet";
  type?: "button" | "submit";
};

export function Button({ href, children, variant = "primary", type = "button" }: ButtonProps) {
  const className = [
    "focus-ring inline-flex min-h-11 items-center justify-center rounded-md px-4 py-2 text-sm font-semibold transition",
    variant === "primary" && "bg-leaf text-white hover:bg-ink",
    variant === "secondary" && "border border-ink/15 bg-white text-ink hover:border-leaf hover:text-leaf",
    variant === "quiet" && "text-ink hover:text-leaf"
  ]
    .filter(Boolean)
    .join(" ");

  if (href) {
    return (
      <Link className={className} href={href}>
        {children}
      </Link>
    );
  }

  return (
    <button className={className} type={type}>
      {children}
    </button>
  );
}

export function Shell({ children, area = "public" }: { children: ReactNode; area?: "public" | "donor" | "admin" }) {
  const nav =
    area === "admin"
      ? [
          ["Dashboard", "/admin"],
          ["Users", "/admin/users"],
          ["Campaigns", "/admin/campaigns"],
          ["Projects", "/admin/projects"],
          ["Reports", "/admin/reports"]
        ]
      : [
          ["Campaigns", "/campaigns"],
          ["Projects", "/projects"],
          ["Donate", "/donate"],
          ["Dashboard", "/dashboard"],
          ["Profile", "/profile"]
        ];

  return (
    <div className="min-h-screen bg-cloud">
      <header className="sticky top-0 z-20 border-b border-ink/10 bg-white/95 backdrop-blur">
        <div className="mx-auto flex max-w-7xl items-center justify-between gap-4 px-4 py-3 sm:px-6 lg:px-8">
          <Link className="text-base font-bold text-ink" href="/">
            Non-Profit Fund Manager
          </Link>
          <nav className="hidden items-center gap-1 md:flex">
            {nav.map(([label, href]) => (
              <Link className="rounded-md px-3 py-2 text-sm font-medium text-ink/75 hover:bg-mint hover:text-ink" href={href} key={href}>
                {label}
              </Link>
            ))}
          </nav>
          <Button href="/login" variant="secondary">
            Sign in
          </Button>
        </div>
      </header>
      <main>{children}</main>
    </div>
  );
}

export function Section({ children, className = "" }: { children: ReactNode; className?: string }) {
  return <section className={`mx-auto max-w-7xl px-4 py-10 sm:px-6 lg:px-8 ${className}`}>{children}</section>;
}

export function Card({ children, className = "" }: { children: ReactNode; className?: string }) {
  return <div className={`rounded-lg border border-ink/10 bg-white p-5 shadow-soft ${className}`}>{children}</div>;
}

export function Stat({ label, value }: { label: string; value: string }) {
  return (
    <div className="rounded-lg border border-ink/10 bg-white p-5">
      <p className="text-sm font-medium text-ink/60">{label}</p>
      <p className="mt-2 text-2xl font-bold text-ink">{value}</p>
    </div>
  );
}

export function ProgressBar({ value }: { value: number }) {
  return (
    <div className="h-2 w-full overflow-hidden rounded-full bg-ink/10">
      <div className="h-full rounded-full bg-coral" style={{ width: `${value}%` }} />
    </div>
  );
}

