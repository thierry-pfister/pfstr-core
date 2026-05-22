import Link from "next/link";
import { api } from "@/lib/api";

export default async function ProjectsPage() {
  const projects = await api.projects.list();

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-xl font-semibold">Projects</h1>
        <Link
          href="/projects/new"
          className="rounded bg-zinc-900 px-4 py-2 text-sm text-white hover:bg-zinc-700 dark:bg-zinc-100 dark:text-zinc-900 dark:hover:bg-zinc-300"
        >
          New project
        </Link>
      </div>
      {projects.length === 0 ? (
        <p className="text-sm text-zinc-500">No projects yet.</p>
      ) : (
        <ul className="divide-y divide-zinc-100 dark:divide-zinc-800">
          {projects.map((project) => (
            <li key={project.id} className="py-3 flex items-center justify-between">
              <div>
                <Link
                  href={`/projects/${project.id}`}
                  className="text-sm font-medium hover:underline"
                >
                  {project.title}
                </Link>
                <p className="text-xs text-zinc-500 mt-0.5">{project.slug}</p>
              </div>
              <span className="text-xs text-zinc-400">{project.status}</span>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
