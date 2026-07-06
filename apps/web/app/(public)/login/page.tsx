import { Button, Card, Section, Shell } from "@/components/ui";

export default function LoginPage() {
  return (
    <Shell>
      <Section className="grid min-h-[70vh] place-items-center">
        <Card className="w-full max-w-md">
          <h1 className="text-3xl font-bold">Sign in</h1>
          <p className="mt-3 text-sm leading-6 text-ink/70">Authentication is handled by Azure AD B2C in production.</p>
          <div className="mt-6 grid gap-3">
            <Button href="/dashboard">Continue with Azure AD B2C</Button>
            <Button href="/register" variant="secondary">
              Create donor account
            </Button>
            <Button href="/forgot-password" variant="quiet">
              Forgot password
            </Button>
          </div>
        </Card>
      </Section>
    </Shell>
  );
}

