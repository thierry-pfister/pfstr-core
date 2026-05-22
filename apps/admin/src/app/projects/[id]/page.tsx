import { notFound } from "next/navigation";
import { api } from "@/lib/api";
import ProjectForm from "@/components/project-form";

export default async function EditProjectPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;

  const projects = await api.projects.list();
  const project = projects.find((p) => p.id === id);
  if (!project) notFound();

  return (
    <div>
      <div className="mb-6">
        <h1 className="text-xl font-semibold">{project.title}</h1>
        <p className="text-xs text-zinc-500 mt-1">{project.slug} · {project.status}</p>
      </div>
      <ProjectForm project={project} />
    </div>
  );
}
