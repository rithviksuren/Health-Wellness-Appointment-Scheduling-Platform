import { Button, Card, Section, Shell } from "@/components/ui";

export default async function ReceiptPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;

  return (
    <Shell area="donor">
      <Section>
        <h1 className="text-4xl font-bold">Receipt</h1>
        <Card className="mt-8 max-w-2xl">
          <p className="text-sm text-ink/60">Receipt reference</p>
          <p className="mt-2 text-2xl font-bold">R-{id.slice(0, 8).toUpperCase()}</p>
          <p className="mt-4 text-ink/70">The generated PDF is stored in Azure Blob Storage and can be resent by email.</p>
          <div className="mt-6 flex gap-3">
            <Button href="#">Download PDF</Button>
            <Button href="#" variant="secondary">
              Resend
            </Button>
          </div>
        </Card>
      </Section>
    </Shell>
  );
}

