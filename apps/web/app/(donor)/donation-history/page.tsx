import { Card, Section, Shell } from "@/components/ui";
import { api } from "@/lib/api";
import { money } from "@/lib/format";

export default async function DonationHistoryPage() {
  const donations = await api.donations();

  return (
    <Shell area="donor">
      <Section>
        <h1 className="text-4xl font-bold">Donation History</h1>
        <Card className="mt-8 overflow-x-auto">
          <table className="w-full border-collapse text-left text-sm">
            <thead>
              <tr className="border-b border-ink/10">
                <th className="py-3">Date</th>
                <th>Amount</th>
                <th>Status</th>
                <th>Receipt</th>
              </tr>
            </thead>
            <tbody>
              {donations.map((donation) => (
                <tr className="border-b border-ink/10 last:border-0" key={donation.id}>
                  <td className="py-3">{new Date(donation.createdAt).toLocaleDateString()}</td>
                  <td>{money(donation.amount, donation.currency)}</td>
                  <td>{donation.status}</td>
                  <td>
                    <a className="font-semibold text-leaf" href={`/receipts/${donation.id}`}>
                      View
                    </a>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </Card>
      </Section>
    </Shell>
  );
}

