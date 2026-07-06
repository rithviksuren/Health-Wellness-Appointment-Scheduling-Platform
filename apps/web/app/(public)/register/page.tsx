import { Button, Card, Section, Shell } from "@/components/ui";

export default function RegisterPage() {
  return (
    <Shell>
      <Section className="grid min-h-[70vh] place-items-center">
        <Card className="w-full max-w-md">
          <h1 className="text-3xl font-bold">Register</h1>
          <p className="mt-3 text-sm leading-6 text-ink/70">New donor registration redirects to the Azure AD B2C sign-up policy.</p>
          <div className="mt-6">
            <Button href="/dashboard">Create Account</Button>
          </div>
        </Card>
      </Section>
    </Shell>
  );
}

