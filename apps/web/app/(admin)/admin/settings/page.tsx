import { Card, Section, Shell } from "@/components/ui";

export default function AdminSettingsPage() {
  return (
    <Shell area="admin">
      <Section>
        <h1 className="text-4xl font-bold">Admin Settings</h1>
        <Card className="mt-8 max-w-2xl">
          <h2 className="text-xl font-bold">Integrations</h2>
          <p className="mt-2 text-ink/70">Configure payment provider, Azure Communication Services, Redis, Blob Storage, and report export policies through environment settings and Key Vault.</p>
        </Card>
      </Section>
    </Shell>
  );
}

