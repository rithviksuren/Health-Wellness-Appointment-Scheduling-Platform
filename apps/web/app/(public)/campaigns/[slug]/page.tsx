import Image from "next/image";
import { Button, Section, Shell, ProgressBar } from "@/components/ui";
import { api } from "@/lib/api";
import { money, percent } from "@/lib/format";

export default async function CampaignDetailPage({ params }: { params: Promise<{ slug: string }> }) {
  const { slug } = await params;
  const campaign = await api.campaign(slug);
  const progress = percent(campaign.raisedAmount, campaign.goalAmount);

  return (
    <Shell>
      <section className="relative h-[420px] bg-ink text-white">
        <Image
          alt=""
          className="object-cover opacity-55"
          fill
          priority
          sizes="100vw"
          src={campaign.heroImageUrl ?? "https://images.unsplash.com/photo-1488521787991-ed7bbaae773c?auto=format&fit=crop&w=1800&q=80"}
        />
        <div className="relative mx-auto flex h-full max-w-7xl flex-col justify-end px-4 pb-10 sm:px-6 lg:px-8">
          <p className="text-sm font-bold uppercase text-mint">{campaign.status}</p>
          <h1 className="mt-3 max-w-3xl text-5xl font-bold">{campaign.name}</h1>
          <p className="mt-4 max-w-2xl text-lg text-white/85">{campaign.summary}</p>
        </div>
      </section>
      <Section className="grid gap-8 lg:grid-cols-[1fr_360px]">
        <article className="prose prose-lg max-w-none">
          <h2>Campaign Story</h2>
          <p>
            Donations to this campaign are tracked from gift through payment, receipt, allocation, reporting, and donor communication. Administrators can publish updates, monitor progress, and export finance-ready summaries.
          </p>
          <p>
            Every gift supports project-level fund allocation, transparent reporting, and timely donor receipts.
          </p>
        </article>
        <aside className="rounded-lg border border-ink/10 bg-white p-5 shadow-soft">
          <ProgressBar value={progress} />
          <div className="mt-4 flex justify-between text-sm">
            <strong>{money(campaign.raisedAmount)}</strong>
            <span className="text-ink/60">{money(campaign.goalAmount)} goal</span>
          </div>
          <Button href={`/donate?campaign=${campaign.slug}`}>Donate to this Campaign</Button>
        </aside>
      </Section>
    </Shell>
  );
}
