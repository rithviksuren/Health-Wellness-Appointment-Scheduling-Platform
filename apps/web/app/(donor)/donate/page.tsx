import { Button, Card, Section, Shell } from "@/components/ui";
import { api } from "@/lib/api";

export default async function DonatePage() {
  const campaigns = await api.campaigns();

  return (
    <Shell area="donor">
      <Section className="grid gap-8 lg:grid-cols-[1fr_420px]">
        <div>
          <h1 className="text-4xl font-bold">Donate</h1>
          <p className="mt-3 max-w-2xl text-ink/70">Create one-time or recurring donations and receive automated receipts.</p>
        </div>
        <Card>
          <form className="grid gap-4">
            <label className="grid gap-2 text-sm font-semibold">
              Campaign
              <select className="rounded-md border border-ink/15 px-3 py-3">
                {campaigns.map((campaign) => (
                  <option key={campaign.id}>{campaign.name}</option>
                ))}
              </select>
            </label>
            <label className="grid gap-2 text-sm font-semibold">
              Amount
              <input className="rounded-md border border-ink/15 px-3 py-3" defaultValue="100" min="1" type="number" />
            </label>
            <label className="grid gap-2 text-sm font-semibold">
              Frequency
              <select className="rounded-md border border-ink/15 px-3 py-3">
                <option>One-time</option>
                <option>Monthly</option>
                <option>Quarterly</option>
                <option>Annual</option>
              </select>
            </label>
            <label className="flex items-center gap-2 text-sm">
              <input type="checkbox" defaultChecked />
              Generate receipt automatically
            </label>
            <Button type="submit">Continue to Payment</Button>
          </form>
        </Card>
      </Section>
    </Shell>
  );
}

