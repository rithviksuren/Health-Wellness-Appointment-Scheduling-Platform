import Image from "next/image";
import { Button, Card, ProgressBar } from "@/components/ui";
import { money, percent } from "@/lib/format";
import type { Campaign } from "@/types/contracts";

export function CampaignCard({ campaign }: { campaign: Campaign }) {
  const progress = percent(campaign.raisedAmount, campaign.goalAmount);

  return (
    <Card className="overflow-hidden p-0">
      <div className="relative h-44 w-full">
        <Image
          alt=""
          className="object-cover"
          fill
          sizes="(min-width: 768px) 33vw, 100vw"
          src={campaign.heroImageUrl ?? "https://images.unsplash.com/photo-1488521787991-ed7bbaae773c?auto=format&fit=crop&w=1200&q=80"}
        />
      </div>
      <div className="p-5">
        <p className="text-xs font-bold uppercase text-leaf">{campaign.status}</p>
        <h3 className="mt-2 text-xl font-bold">{campaign.name}</h3>
        <p className="mt-2 min-h-14 text-sm leading-6 text-ink/70">{campaign.summary}</p>
        <div className="mt-4 space-y-2">
          <ProgressBar value={progress} />
          <div className="flex items-center justify-between text-sm">
            <span className="font-semibold">{money(campaign.raisedAmount)}</span>
            <span className="text-ink/60">{progress}% of {money(campaign.goalAmount)}</span>
          </div>
        </div>
        <div className="mt-5 flex gap-2">
          <Button href={`/campaigns/${campaign.slug}`} variant="secondary">
            View
          </Button>
          <Button href={`/donate?campaign=${campaign.slug}`}>Donate</Button>
        </div>
      </div>
    </Card>
  );
}
