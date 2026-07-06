import { CampaignCard } from "@/components/campaign-card";
import { Section, Shell } from "@/components/ui";
import { api } from "@/lib/api";

export default async function CampaignsPage() {
  const campaigns = await api.campaigns();

  return (
    <Shell>
      <Section>
        <h1 className="text-4xl font-bold">Campaigns</h1>
        <p className="mt-3 max-w-2xl text-ink/70">Track goals, funding progress, and campaign impact across the organization.</p>
        <div className="mt-8 grid gap-5 md:grid-cols-3">
          {campaigns.map((campaign) => (
            <CampaignCard campaign={campaign} key={campaign.id} />
          ))}
        </div>
      </Section>
    </Shell>
  );
}

