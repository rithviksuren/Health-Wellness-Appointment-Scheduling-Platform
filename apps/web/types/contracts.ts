export type Role = "Donor" | "Volunteer" | "Treasurer" | "Campaign Manager" | "Admin";

export type Campaign = {
  id: string;
  name: string;
  slug: string;
  summary: string;
  goalAmount: number;
  raisedAmount: number;
  status: string;
  startsOn: string;
  endsOn?: string | null;
  heroImageUrl?: string | null;
};

export type Project = {
  id: string;
  name: string;
  code: string;
  description: string;
  fundingGoal: number;
  allocatedAmount: number;
  isActive: boolean;
};

export type Donation = {
  id: string;
  donorId: string;
  campaignId?: string | null;
  amount: number;
  currency: string;
  status: string;
  createdAt: string;
};

export type DashboardMetric = {
  label: string;
  value: number;
  format: "currency" | "number" | string;
};

export type Dashboard = {
  metrics: DashboardMetric[];
  campaigns: Campaign[];
  recentDonations: Donation[];
};

export type User = {
  id: string;
  email: string;
  displayName: string;
  roles: Role[];
  isActive: boolean;
};

