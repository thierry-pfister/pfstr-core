"use client";

export default function ErrorPage({
  error,
  unstable_retry,
}: {
  error: Error & { digest?: string };
  unstable_retry: () => void;
}) {
  return (
    <div className="flex flex-col gap-3 py-12">
      <p className="text-sm font-medium text-red-600 dark:text-red-400">Something went wrong</p>
      <p className="text-sm text-zinc-500">{error.message}</p>
      <button
        onClick={unstable_retry}
        className="self-start rounded border border-zinc-200 px-3 py-1.5 text-sm hover:bg-zinc-50 dark:border-zinc-700 dark:hover:bg-zinc-800"
      >
        Try again
      </button>
    </div>
  );
}
