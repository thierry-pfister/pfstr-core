"use client";

import { useTransition } from "react";
import { updateProject, publishProject, archiveProject } from "@/lib/actions/projects";
import type { Project } from "@/lib/types";

export default function ProjectForm({ project }: { project: Project }) {
  const [pending, startTransition] = useTransition();

  const base =
    "w-full rounded border border-zinc-200 bg-white px-3 py-2 text-sm dark:border-zinc-700 dark:bg-zinc-900 focus:outline-none focus:ring-2 focus:ring-zinc-400";

  return (
    <div className="max-w-xl flex flex-col gap-6">
      <form
        action={(fd) => startTransition(() => updateProject(project.id, fd))}
        className="flex flex-col gap-4"
      >
        <div className="flex flex-col gap-1">
          <label className="text-sm font-medium" htmlFor="title">Title</label>
          <input id="title" name="title" defaultValue={project.title} required className={base} />
        </div>
        <div className="flex flex-col gap-1">
          <label className="text-sm font-medium" htmlFor="summary">Summary</label>
          <textarea id="summary" name="summary" defaultValue={project.summary} required rows={3} className={base} />
        </div>
        <div className="flex flex-col gap-1">
          <label className="text-sm font-medium" htmlFor="content">Content</label>
          <textarea id="content" name="content" defaultValue={project.content ?? ""} rows={10} className={base} />
        </div>
        <div className="flex flex-col gap-1">
          <label className="text-sm font-medium" htmlFor="techStack">Tech stack (comma-separated)</label>
          <input id="techStack" name="techStack" defaultValue={project.techStack.join(", ")} className={base} />
        </div>
        <div className="flex flex-col gap-1">
          <label className="text-sm font-medium" htmlFor="displayOrder">Display order</label>
          <input id="displayOrder" name="displayOrder" type="number" defaultValue={project.displayOrder} className={base} />
        </div>
        <button
          type="submit"
          disabled={pending}
          className="self-start rounded bg-zinc-900 px-4 py-2 text-sm text-white hover:bg-zinc-700 dark:bg-zinc-100 dark:text-zinc-900 disabled:opacity-50"
        >
          {pending ? "Saving…" : "Save"}
        </button>
      </form>

      <div className="flex gap-2 pt-2 border-t border-zinc-100 dark:border-zinc-800">
        {project.status === "Active" && (
          <button
            onClick={() => startTransition(() => publishProject(project.id))}
            disabled={pending}
            className="rounded bg-green-600 px-4 py-2 text-sm text-white hover:bg-green-500 disabled:opacity-50"
          >
            Publish
          </button>
        )}
        {project.status !== "Archived" && (
          <button
            onClick={() => startTransition(() => archiveProject(project.id))}
            disabled={pending}
            className="rounded bg-red-600 px-4 py-2 text-sm text-white hover:bg-red-500 disabled:opacity-50"
          >
            Archive
          </button>
        )}
      </div>
    </div>
  );
}
