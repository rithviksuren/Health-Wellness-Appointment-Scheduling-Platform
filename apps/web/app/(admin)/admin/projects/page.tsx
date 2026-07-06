import { Card, Section, Shell } from "@/components/ui";
import { api } from "@/lib/api";
import { money } from "@/lib/format";

export default async function AdminProjectsPage() {
  const projects = await api.projects();

  return (
    <Shell area="admin">
      <Section>
        <h1 className="text-4xl font-bold">Project Funding</h1>
        <div className="mt-8 grid gap-5 md:grid-cols-3">
          {projects.map((project) => (
            <Card key={project.id}>
              <h2 className="text-xl font-bold">{project.name}</h2>
              <p className="mt-2 text-sm text-ink/70">{project.description}</p>
              <p className="mt-4 font-semibold">
                {money(project.allocatedAmount)} allocated of {money(project.fundingGoal)}
              </p>
            </Card>
          ))}
        </div>
      </Section>
    </Shell>
  );
}

