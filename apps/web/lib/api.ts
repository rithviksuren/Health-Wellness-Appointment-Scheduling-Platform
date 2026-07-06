import { campaigns, dashboard, donations, projects, users } from "@/lib/sample-data";
import type { Campaign, Dashboard, Donation, Project, User } from "@/types/contracts";

const apiBaseUrl = process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://127.0.0.1:8000";

async function getJson<T>(path: string, fallback: T): Promise<T> {
  if (!apiBaseUrl) {
    return fallback;
  }

  try {
    const response = await fetch(`${apiBaseUrl}${path}`, {
      headers: {
        accept: "application/json"
      },
      cache: "no-store"
    });

    if (!response.ok) {
      return fallback;
    }

    return response.json() as Promise<T>;
  } catch {
    return fallback;
  }
}

export const api = {
  campaigns: () => getJson<Campaign[]>("/api/campaigns", campaigns),
  campaign: async (slug: string) => {
    const fallback = campaigns.find((campaign) => campaign.slug === slug) ?? campaigns[0];
    return getJson<Campaign>(`/api/campaigns/${slug}`, fallback);
  },
  projects: () => getJson<Project[]>("/api/projects", projects),
  donations: () => getJson<Donation[]>("/api/donations/me", donations),
  donorDashboard: () => getJson<Dashboard>("/api/dashboard/donor", dashboard),
  adminDashboard: () => getJson<Dashboard>("/api/dashboard/admin", dashboard),
  users: () => getJson<User[]>("/api/users", users)
};
