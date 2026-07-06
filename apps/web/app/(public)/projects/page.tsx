import { Card, ProgressBar, Section, Shell } from "@/components/ui";
import { api } from "@/lib/api";
import { money, percent } from "@/lib/format";

export default async function ProjectsPage() {
  const projects = await api.projects();

  return (
    <Shell>
      <Section>
        <h1 className="text-4xl font-bold">Projects</h1>
        <p className="mt-3 max-w-2xl text-ink/70">Allocate donated funds to programs and track progress against funding goals.</p>
        <div className="mt-8 grid gap-5 md:grid-cols-3">
          {projects.map((project) => (
            <Card key={project.id}>
              <p className="text-xs font-bold uppercase text-leaf">{project.code}</p>
              <h2 className="mt-2 text-xl font-bold">{project.name}</h2>
              <p className="mt-2 min-h-20 text-sm leading-6 text-ink/70">{project.description}</p>
              <div className="mt-5 space-y-2">
                <ProgressBar value={percent(project.allocatedAmount, project.fundingGoal)} />
                <div className="flex justify-between text-sm">
                  <strong>{money(project.allocatedAmount)}</strong>
                  <span>{money(project.fundingGoal)}</span>
                </div>
              </div>
            </Card>
          ))}
        </div>
      </Section>
    </Shell>
  );
}

