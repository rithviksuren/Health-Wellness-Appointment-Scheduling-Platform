import { Card, Section, Shell } from "@/components/ui";

export default function SettingsPage() {
  return (
    <Shell area="donor">
      <Section>
        <h1 className="text-4xl font-bold">Settings</h1>
        <Card className="mt-8 max-w-2xl">
          <h2 className="text-xl font-bold">Security</h2>
          <p className="mt-2 text-ink/70">Account security, MFA, and password changes are managed by Azure AD B2C policies.</p>
        </Card>
      </Section>
    </Shell>
  );
}

