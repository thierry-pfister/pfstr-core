"use client";

export default function GlobalError({
  unstable_retry,
}: {
  error: Error & { digest?: string };
  unstable_retry: () => void;
}) {
  return (
    <html>
      <body className="flex items-center justify-center h-screen bg-white dark:bg-zinc-950 text-zinc-900 dark:text-zinc-100">
        <div className="flex flex-col gap-3 text-center">
          <p className="text-sm font-medium text-red-600 dark:text-red-400">Something went wrong</p>
          <button
            onClick={unstable_retry}
            className="self-center rounded border border-zinc-200 px-3 py-1.5 text-sm hover:bg-zinc-50 dark:border-zinc-700"
          >
            Try again
          </button>
        </div>
      </body>
    </html>
  );
}
