import Image from "next/image";
import { CampaignCard } from "@/components/campaign-card";
import { Button, Section, Shell, Stat } from "@/components/ui";
import { api } from "@/lib/api";
import { money } from "@/lib/format";

export default async function HomePage() {
  const [campaigns, dashboard] = await Promise.all([api.campaigns(), api.donorDashboard()]);

  return (
    <Shell>
      <section className="relative overflow-hidden bg-ink text-white">
        <Image
          alt=""
          className="object-cover opacity-50"
          fill
          priority
          sizes="100vw"
          src="https://images.unsplash.com/photo-1488521787991-ed7bbaae773c?auto=format&fit=crop&w=1800&q=80"
        />
        <div className="relative mx-auto grid min-h-[620px] max-w-7xl content-end px-4 pb-14 pt-28 sm:px-6 lg:px-8">
          <div className="max-w-3xl">
            <p className="text-sm font-bold uppercase tracking-wide text-mint">Azure-ready donation operations</p>
            <h1 className="mt-4 text-5xl font-bold leading-tight sm:text-6xl">Non-Profit Fund Manager</h1>
            <p className="mt-5 max-w-2xl text-lg leading-8 text-white/85">
              Manage campaigns, recurring donations, receipts, project allocations, donor communication, and financial reporting from one secure workspace.
            </p>
            <div className="mt-8 flex flex-wrap gap-3">
              <Button href="/donate">Start a Donation</Button>
              <Button href="/campaigns" variant="secondary">
                Explore Campaigns
              </Button>
            </div>
          </div>
        </div>
      </section>
      <Section className="-mt-8 grid gap-4 md:grid-cols-4">
        {dashboard.metrics.map((metric) => (
          <Stat key={metric.label} label={metric.label} value={metric.format === "currency" ? money(metric.value) : metric.value.toLocaleString()} />
        ))}
      </Section>
      <Section>
        <div className="mb-6 flex items-end justify-between gap-4">
          <div>
            <p className="text-sm font-bold uppercase text-leaf">Active campaigns</p>
            <h2 className="mt-2 text-3xl font-bold">Fund work with measurable outcomes</h2>
          </div>
          <Button href="/campaigns" variant="secondary">
            All Campaigns
          </Button>
        </div>
        <div className="grid gap-5 md:grid-cols-3">
          {campaigns.map((campaign) => (
            <CampaignCard campaign={campaign} key={campaign.id} />
          ))}
        </div>
      </Section>
    </Shell>
  );
}
