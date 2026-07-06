import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "Non-Profit Fund Manager",
  description: "Donation, campaign, and fund management for mission-driven organizations."
};

export default function RootLayout({ children }: Readonly<{ children: React.ReactNode }>) {
  return (
    <html lang="en">
      <body>{children}</body>
    </html>
  );
}

