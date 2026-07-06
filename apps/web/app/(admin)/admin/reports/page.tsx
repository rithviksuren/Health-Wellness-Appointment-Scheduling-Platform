import { Card, Section, Shell, Stat } from "@/components/ui";
import { dashboard } from "@/lib/sample-data";
import { money } from "@/lib/format";

export default function ReportsPage() {
  return (
    <Shell area="admin">
      <Section>
        <h1 className="text-4xl font-bold">Financial Reports</h1>
        <div className="mt-8 grid gap-4 md:grid-cols-4">
          {dashboard.metrics.map((metric) => (
            <Stat key={metric.label} label={metric.label} value={metric.format === "currency" ? money(metric.value) : metric.value.toLocaleString()} />
          ))}
        </div>
        <div className="mt-8 grid gap-5 lg:grid-cols-2">
          <Card>
            <h2 className="text-xl font-bold">Monthly Donation Trend</h2>
            <div className="mt-6 flex h-52 items-end gap-3">
              {[42, 58, 51, 73, 69, 88].map((height, index) => (
                <div className="flex-1 rounded-t-md bg-leaf" key={index} style={{ height: `${height}%` }} />
              ))}
            </div>
          </Card>
          <Card>
            <h2 className="text-xl font-bold">KPI Coverage</h2>
            <ul className="mt-4 grid gap-3 text-sm text-ink/75">
              <li>Donation summary</li>
              <li>Campaign performance</li>
              <li>Donor retention</li>
              <li>Project funding progress</li>
              <li>Payment failure rate</li>
            </ul>
          </Card>
        </div>
      </Section>
    </Shell>
  );
}

