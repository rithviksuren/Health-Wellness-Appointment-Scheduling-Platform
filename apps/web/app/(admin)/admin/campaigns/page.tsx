import { CampaignCard } from "@/components/campaign-card";
import { Button, Section, Shell } from "@/components/ui";
import { api } from "@/lib/api";

export default async function AdminCampaignsPage() {
  const campaigns = await api.campaigns();

  return (
    <Shell area="admin">
      <Section>
        <div className="flex items-center justify-between gap-4">
          <h1 className="text-4xl font-bold">Campaign Management</h1>
          <Button href="/admin/campaigns/new">New Campaign</Button>
        </div>
        <div className="mt-8 grid gap-5 md:grid-cols-3">
          {campaigns.map((campaign) => (
            <CampaignCard campaign={campaign} key={campaign.id} />
          ))}
        </div>
      </Section>
    </Shell>
  );
}

