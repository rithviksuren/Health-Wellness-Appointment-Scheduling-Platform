import { Card, Section, Shell, Stat } from "@/components/ui";
import { api } from "@/lib/api";
import { money } from "@/lib/format";

export default async function DonorDashboardPage() {
  const dashboard = await api.donorDashboard();

  return (
    <Shell area="donor">
      <Section>
        <h1 className="text-4xl font-bold">Donor Dashboard</h1>
        <div className="mt-8 grid gap-4 md:grid-cols-4">
          {dashboard.metrics.map((metric) => (
            <Stat key={metric.label} label={metric.label} value={metric.format === "currency" ? money(metric.value) : metric.value.toLocaleString()} />
          ))}
        </div>
        <div className="mt-8 grid gap-5 lg:grid-cols-2">
          <Card>
            <h2 className="text-xl font-bold">Recent Donations</h2>
            <div className="mt-4 divide-y divide-ink/10">
              {dashboard.recentDonations.map((donation) => (
                <div className="flex items-center justify-between py-3" key={donation.id}>
                  <span className="text-sm text-ink/70">{new Date(donation.createdAt).toLocaleDateString()}</span>
                  <strong>{money(donation.amount, donation.currency)}</strong>
                </div>
              ))}
            </div>
          </Card>
          <Card>
            <h2 className="text-xl font-bold">Campaign Impact</h2>
            <div className="mt-4 space-y-3">
              {dashboard.campaigns.map((campaign) => (
                <div className="rounded-md bg-cloud p-3" key={campaign.id}>
                  <div className="flex items-center justify-between gap-3">
                    <span className="font-semibold">{campaign.name}</span>
                    <span className="text-sm text-ink/60">{money(campaign.raisedAmount)}</span>
                  </div>
                </div>
              ))}
            </div>
          </Card>
        </div>
      </Section>
    </Shell>
  );
}

