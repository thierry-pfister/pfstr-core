"use client";

import { useTransition, useState } from "react";
import { updatePost, publishPost, archivePost } from "@/lib/actions/posts";
import type { Post } from "@/lib/types";
import MarkdownEditor from "./markdown-editor";

export default function PostForm({ post }: { post: Post }) {
  const [pending, startTransition] = useTransition();
  const [error, setError] = useState<string | null>(null);

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
      <form action={(fd) => run(() => updatePost(post.id, fd))} className="flex flex-col gap-4">
        <div className="flex flex-col gap-1">
          <label className="text-sm font-medium" htmlFor="title">Title</label>
          <input id="title" name="title" defaultValue={post.title} required className={base} />
        </div>
        <div className="flex flex-col gap-1">
          <label className="text-sm font-medium" htmlFor="summary">Summary</label>
          <textarea id="summary" name="summary" defaultValue={post.summary} required rows={3} className={base} />
        </div>
        <div className="flex flex-col gap-1">
          <label className="text-sm font-medium">Content</label>
          <MarkdownEditor name="content" defaultValue={post.content ?? ""} />
        </div>
        <div className="flex flex-col gap-1">
          <label className="text-sm font-medium" htmlFor="tags">Tags (comma-separated)</label>
          <input id="tags" name="tags" defaultValue={post.tags.join(", ")} className={base} />
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
        {post.status === "Draft" && (
          <button
            onClick={() => run(() => publishPost(post.id))}
            disabled={pending}
            className="rounded bg-green-600 px-4 py-2 text-sm text-white hover:bg-green-500 disabled:opacity-50"
          >
            Publish
          </button>
        )}
        {post.status !== "Archived" && (
          <button
            onClick={() => run(() => archivePost(post.id))}
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
