"use client";

import { useState } from "react";
import { marked } from "marked";

export default function MarkdownEditor({
  name,
  defaultValue = "",
}: {
  name: string;
  defaultValue?: string;
}) {
  const [value, setValue] = useState(defaultValue);
  const [tab, setTab] = useState<"write" | "preview">("write");

  const base =
    "w-full rounded-b border border-t-0 border-zinc-200 bg-white px-3 py-2 text-sm dark:border-zinc-700 dark:bg-zinc-900 focus:outline-none focus:ring-2 focus:ring-zinc-400";

  return (
    <div className="flex flex-col">
      <div className="flex border border-zinc-200 dark:border-zinc-700 rounded-t overflow-hidden">
        <TabBtn active={tab === "write"} onClick={() => setTab("write")}>Write</TabBtn>
        <TabBtn active={tab === "preview"} onClick={() => setTab("preview")}>Preview</TabBtn>
      </div>
      {tab === "write" ? (
        <textarea
          name={name}
          value={value}
          onChange={(e) => setValue(e.target.value)}
          rows={14}
          className={base}
          placeholder="Markdown supported"
        />
      ) : (
        <div
          className={`${base} min-h-[224px] prose prose-sm dark:prose-invert max-w-none`}
          dangerouslySetInnerHTML={{ __html: marked(value) as string }}
        />
      )}
    </div>
  );
}

function TabBtn({
  active,
  onClick,
  children,
}: {
  active: boolean;
  onClick: () => void;
  children: React.ReactNode;
}) {
  return (
    <button
      type="button"
      onClick={onClick}
      className={`px-4 py-1.5 text-xs font-medium ${
        active
          ? "bg-zinc-900 text-white dark:bg-zinc-100 dark:text-zinc-900"
          : "text-zinc-500 hover:text-zinc-800 dark:hover:text-zinc-200"
      }`}
    >
      {children}
    </button>
  );
}
