"use client";

import { useTransition, useState } from "react";
import { createPost } from "@/lib/actions/posts";
import { slugify } from "@/lib/slugify";

export default function NewPostPage() {
  const [pending, startTransition] = useTransition();
  const [slug, setSlug] = useState("");
  const [slugTouched, setSlugTouched] = useState(false);

  const base =
    "w-full rounded border border-zinc-200 bg-white px-3 py-2 text-sm dark:border-zinc-700 dark:bg-zinc-900 focus:outline-none focus:ring-2 focus:ring-zinc-400";

  function handleTitleChange(e: React.ChangeEvent<HTMLInputElement>) {
    if (!slugTouched) setSlug(slugify(e.target.value));
  }

  return (
    <div className="max-w-xl">
      <h1 className="text-xl font-semibold mb-6">New post</h1>
      <form
        action={(fd) => startTransition(() => createPost(fd))}
        className="flex flex-col gap-4"
      >
        <div className="flex flex-col gap-1">
          <label className="text-sm font-medium" htmlFor="title">Title</label>
          <input
            id="title"
            name="title"
            required
            onChange={handleTitleChange}
            className={base}
          />
        </div>
        <div className="flex flex-col gap-1">
          <label className="text-sm font-medium" htmlFor="slug">Slug</label>
          <input
            id="slug"
            name="slug"
            required
            value={slug}
            onChange={(e) => { setSlugTouched(true); setSlug(e.target.value); }}
            className={base}
          />
        </div>
        <div className="flex flex-col gap-1">
          <label className="text-sm font-medium" htmlFor="summary">Summary</label>
          <textarea id="summary" name="summary" required rows={3} className={base} />
        </div>
        <button
          type="submit"
          disabled={pending}
          className="self-start rounded bg-zinc-900 px-4 py-2 text-sm text-white hover:bg-zinc-700 dark:bg-zinc-100 dark:text-zinc-900 disabled:opacity-50"
        >
          {pending ? "Creating…" : "Create"}
        </button>
      </form>
    </div>
  );
}
