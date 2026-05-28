"use client";

import { useTransition, useState } from "react";
import { updateProject, publishProject, archiveProject } from "@/lib/actions/projects";
import type { Project } from "@/lib/types";
import MarkdownEditor from "./markdown-editor";

export default function ProjectForm({ project }: { project: Project }) {
  const [pending, startTransition] = useTransition();
  const [error, setError] = useState<string | null>(null);
  const [coverUrl, setCoverUrl] = useState<string>(project.coverImageUrl ?? "");
  const [uploading, setUploading] = useState(false);

  async function handleFileUpload(e: React.ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0];
    if (!file) return;
    setUploading(true);
    setError(null);
    try {
      const fd = new FormData();
      fd.append("file", file);
      const res = await fetch("/api/upload", { method: "POST", body: fd });
      if (!res.ok) throw new Error(await res.text());
      const { url } = (await res.json()) as { url: string };
      setCoverUrl(url);
    } catch {
      setError("Upload failed.");
    } finally {
      setUploading(false);
      e.target.value = "";
    }
  }

  const base =
    "w-full rounded border border-zinc-200 bg-white px-3 py-2 text-sm dark:border-zinc-700 dark:bg-zinc-900 focus:outline-none focus:ring-2 focus:ring-zinc-400";

  function run(fn: () => Promise<void>) {
    startTransition(async () => {
      try {
        await fn();
        setError(null);
      } catch {
        setError("Action failed. Is the API running?");
      }
    });
  }

  return (
    <div className="max-w-xl flex flex-col gap-6">
      <form action={(fd) => run(() => updateProject(project.id, fd))} className="flex flex-col gap-4">
        <div className="flex flex-col gap-1">
          <label className="text-sm font-medium" htmlFor="title">Title</label>
          <input id="title" name="title" defaultValue={project.title} required className={base} />
        </div>
        <div className="flex flex-col gap-1">
          <label className="text-sm font-medium" htmlFor="summary">Summary</label>
          <textarea id="summary" name="summary" defaultValue={project.summary} required rows={3} className={base} />
        </div>
        <div className="flex flex-col gap-1">
          <label className="text-sm font-medium">Content</label>
          <MarkdownEditor name="content" defaultValue={project.content ?? ""} />
        </div>
        <div className="flex flex-col gap-1">
          <label className="text-sm font-medium" htmlFor="coverImageUrl">Cover image</label>
          <div className="flex gap-2">
            <input
              id="coverImageUrl"
              name="coverImageUrl"
              type="url"
              value={coverUrl}
              onChange={(e) => setCoverUrl(e.target.value)}
              placeholder="https://… or upload →"
              className={`${base} flex-1`}
            />
            <label className={`cursor-pointer rounded border border-zinc-200 dark:border-zinc-700 px-3 py-2 text-sm whitespace-nowrap hover:bg-zinc-50 dark:hover:bg-zinc-800 ${uploading ? "opacity-50 pointer-events-none" : ""}`}>
              {uploading ? "Uploading…" : "Upload"}
              <input
                type="file"
                accept="image/jpeg,image/png,image/gif,image/webp"
                className="sr-only"
                onChange={handleFileUpload}
                disabled={uploading || pending}
              />
            </label>
          </div>
          {coverUrl && (
            // eslint-disable-next-line @next/next/no-img-element
            <img src={coverUrl} alt="Cover preview" className="mt-1 h-32 w-full rounded object-cover border border-zinc-200 dark:border-zinc-700" />
          )}
        </div>
        <div className="flex flex-col gap-1">
          <label className="text-sm font-medium" htmlFor="techStack">Tech stack (comma-separated)</label>
          <input id="techStack" name="techStack" defaultValue={project.techStack.join(", ")} className={base} />
        </div>
        <div className="flex flex-col gap-1">
          <label className="text-sm font-medium" htmlFor="displayOrder">Display order</label>
          <input id="displayOrder" name="displayOrder" type="number" defaultValue={project.displayOrder} className={base} />
        </div>
        {error && (
          <p className="text-sm text-red-600 dark:text-red-400" aria-live="polite">{error}</p>
        )}
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
            onClick={() => run(() => publishProject(project.id))}
            disabled={pending}
            className="rounded bg-green-600 px-4 py-2 text-sm text-white hover:bg-green-500 disabled:opacity-50"
          >
            Publish
          </button>
        )}
        {project.status !== "Archived" && (
          <button
            onClick={() => run(() => archiveProject(project.id))}
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
