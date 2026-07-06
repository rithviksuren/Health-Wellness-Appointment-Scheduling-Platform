import { Card, Section, Shell, Stat } from "@/components/ui";
import { api } from "@/lib/api";
import { money } from "@/lib/format";

export default async function AdminPage() {
  const dashboard = await api.adminDashboard();

  return (
    <Shell area="admin">
      <Section>
        <h1 className="text-4xl font-bold">Admin Dashboard</h1>
        <div className="mt-8 grid gap-4 md:grid-cols-4">
          {dashboard.metrics.map((metric) => (
            <Stat key={metric.label} label={metric.label} value={metric.format === "currency" ? money(metric.value) : metric.value.toLocaleString()} />
          ))}
        </div>
        <Card className="mt-8">
          <h2 className="text-xl font-bold">Operational Health</h2>
          <div className="mt-4 grid gap-3 md:grid-cols-3">
            {["API healthy", "Receipts queued", "Application Insights connected"].map((item) => (
              <div className="rounded-md bg-mint p-3 text-sm font-semibold" key={item}>
                {item}
              </div>
            ))}
          </div>
        </Card>
      </Section>
    </Shell>
  );
}

