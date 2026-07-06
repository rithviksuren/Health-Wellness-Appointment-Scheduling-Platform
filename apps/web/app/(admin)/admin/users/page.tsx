import { Card, Section, Shell } from "@/components/ui";
import { api } from "@/lib/api";

export default async function AdminUsersPage() {
  const users = await api.users();

  return (
    <Shell area="admin">
      <Section>
        <h1 className="text-4xl font-bold">User Management</h1>
        <Card className="mt-8 overflow-x-auto">
          <table className="w-full text-left text-sm">
            <thead>
              <tr className="border-b border-ink/10">
                <th className="py-3">Name</th>
                <th>Email</th>
                <th>Roles</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              {users.map((user) => (
                <tr className="border-b border-ink/10 last:border-0" key={user.id}>
                  <td className="py-3 font-semibold">{user.displayName}</td>
                  <td>{user.email}</td>
                  <td>{user.roles.join(", ")}</td>
                  <td>{user.isActive ? "Active" : "Disabled"}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </Card>
      </Section>
    </Shell>
  );
}

