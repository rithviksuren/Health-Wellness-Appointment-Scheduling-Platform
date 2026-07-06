import { Button, Card, Section, Shell } from "@/components/ui";

export default function ProfilePage() {
  return (
    <Shell area="donor">
      <Section>
        <h1 className="text-4xl font-bold">Profile</h1>
        <Card className="mt-8 max-w-2xl">
          <form className="grid gap-4">
            <label className="grid gap-2 text-sm font-semibold">
              Display name
              <input className="rounded-md border border-ink/15 px-3 py-3" defaultValue="Ava Patel" />
            </label>
            <label className="grid gap-2 text-sm font-semibold">
              Email
              <input className="rounded-md border border-ink/15 px-3 py-3" defaultValue="ava@example.org" />
            </label>
            <label className="flex items-center gap-2 text-sm">
              <input type="checkbox" defaultChecked />
              Email notifications
            </label>
            <label className="flex items-center gap-2 text-sm">
              <input type="checkbox" />
              SMS notifications
            </label>
            <Button type="submit">Save Profile</Button>
          </form>
        </Card>
      </Section>
    </Shell>
  );
}

