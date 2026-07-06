import type { Campaign, Dashboard, Donation, Project, User } from "@/types/contracts";

export const campaigns: Campaign[] = [
  {
    id: "campaign-clean-water",
    name: "Clean Water Access",
    slug: "clean-water-access",
    summary: "Fund community water filters and maintenance training for rural schools.",
    goalAmount: 85000,
    raisedAmount: 52340,
    status: "Published",
    startsOn: "2026-01-01",
    endsOn: "2026-12-31",
    heroImageUrl: "https://images.unsplash.com/photo-1541544741938-0af808871cc0?auto=format&fit=crop&w=1600&q=80"
  },
  {
    id: "campaign-student-meals",
    name: "Student Meal Fund",
    slug: "student-meal-fund",
    summary: "Provide nutritious school meals and weekend food kits for children.",
    goalAmount: 120000,
    raisedAmount: 91200,
    status: "Published",
    startsOn: "2026-02-01",
    heroImageUrl: "https://images.unsplash.com/photo-1488521787991-ed7bbaae773c?auto=format&fit=crop&w=1600&q=80"
  },
  {
    id: "campaign-clinic-outreach",
    name: "Mobile Clinic Outreach",
    slug: "mobile-clinic-outreach",
    summary: "Expand preventive health visits with mobile clinic teams and supplies.",
    goalAmount: 150000,
    raisedAmount: 48100,
    status: "Published",
    startsOn: "2026-03-01",
    heroImageUrl: "https://images.unsplash.com/photo-1584515933487-779824d29309?auto=format&fit=crop&w=1600&q=80"
  }
];

export const projects: Project[] = [
  {
    id: "project-water-filters",
    name: "Water Filter Installations",
    code: "WATER-2026",
    description: "Install long-life water filters, train local caretakers, and monitor water quality.",
    fundingGoal: 65000,
    allocatedAmount: 38400,
    isActive: true
  },
  {
    id: "project-school-meals",
    name: "School Meals Program",
    code: "MEALS-2026",
    description: "Daily meals, weekend food packs, and nutrition tracking for partner schools.",
    fundingGoal: 90000,
    allocatedAmount: 71200,
    isActive: true
  },
  {
    id: "project-clinic-supplies",
    name: "Mobile Clinic Supplies",
    code: "CLINIC-2026",
    description: "Medical supplies, screening equipment, and transport support for mobile clinics.",
    fundingGoal: 110000,
    allocatedAmount: 32100,
    isActive: true
  }
];

export const donations: Donation[] = [
  { id: "don-001", donorId: "user-001", campaignId: "campaign-clean-water", amount: 250, currency: "USD", status: "Succeeded", createdAt: "2026-07-02T09:00:00Z" },
  { id: "don-002", donorId: "user-002", campaignId: "campaign-student-meals", amount: 75, currency: "USD", status: "Succeeded", createdAt: "2026-07-03T15:30:00Z" },
  { id: "don-003", donorId: "user-003", campaignId: "campaign-clinic-outreach", amount: 500, currency: "USD", status: "Succeeded", createdAt: "2026-07-04T18:45:00Z" }
];

export const users: User[] = [
  { id: "user-001", email: "ava@example.org", displayName: "Ava Patel", roles: ["Donor"], isActive: true },
  { id: "user-002", email: "treasurer@example.org", displayName: "Noah Williams", roles: ["Treasurer"], isActive: true },
  { id: "user-003", email: "admin@example.org", displayName: "Maya Johnson", roles: ["Admin"], isActive: true }
];

export const dashboard: Dashboard = {
  metrics: [
    { label: "Total Raised", value: 191640, format: "currency" },
    { label: "Active Donors", value: 2840, format: "number" },
    { label: "Recurring MRR", value: 18325, format: "currency" },
    { label: "Receipt SLA", value: 98, format: "number" }
  ],
  campaigns,
  recentDonations: donations
};

