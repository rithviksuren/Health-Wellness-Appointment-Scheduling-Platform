import { Card, Section, Shell } from "@/components/ui";

export default function NotificationsPage() {
  return (
    <Shell area="donor">
      <Section>
        <h1 className="text-4xl font-bold">Notifications</h1>
        <div className="mt-8 grid gap-4">
          {["Receipt generated", "Donation succeeded", "Campaign update published"].map((item) => (
            <Card key={item}>
              <h2 className="font-bold">{item}</h2>
              <p className="mt-1 text-sm text-ink/70">Queued through Azure Communication Services.</p>
            </Card>
          ))}
        </div>
      </Section>
    </Shell>
  );
}

