import { Button, Card, Section, Shell } from "@/components/ui";

export default function ForgotPasswordPage() {
  return (
    <Shell>
      <Section className="grid min-h-[70vh] place-items-center">
        <Card className="w-full max-w-md">
          <h1 className="text-3xl font-bold">Reset password</h1>
          <p className="mt-3 text-sm leading-6 text-ink/70">Password recovery is delegated to the Azure AD B2C password reset policy.</p>
          <div className="mt-6">
            <Button href="/login">Open Reset Flow</Button>
          </div>
        </Card>
      </Section>
    </Shell>
  );
}

