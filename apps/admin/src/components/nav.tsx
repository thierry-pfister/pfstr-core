import Link from "next/link";

export default function Nav() {
  return (
    <nav className="w-48 shrink-0 border-r border-zinc-200 dark:border-zinc-800 px-4 py-6 flex flex-col gap-1">
      <p className="text-xs font-semibold uppercase tracking-widest text-zinc-400 mb-3">pfstr</p>
      <Link
        href="/posts"
        className="rounded px-3 py-2 text-sm text-zinc-700 hover:bg-zinc-100 dark:text-zinc-300 dark:hover:bg-zinc-800"
      >
        Posts
      </Link>
      <Link
        href="/projects"
        className="rounded px-3 py-2 text-sm text-zinc-700 hover:bg-zinc-100 dark:text-zinc-300 dark:hover:bg-zinc-800"
      >
        Projects
      </Link>
    </nav>
  );
}
