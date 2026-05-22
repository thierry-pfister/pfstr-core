import Link from "next/link";
import { api } from "@/lib/api";

export default async function PostsPage() {
  const posts = await api.posts.list();

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-xl font-semibold">Posts</h1>
        <Link
          href="/posts/new"
          className="rounded bg-zinc-900 px-4 py-2 text-sm text-white hover:bg-zinc-700 dark:bg-zinc-100 dark:text-zinc-900 dark:hover:bg-zinc-300"
        >
          New post
        </Link>
      </div>
      {posts.length === 0 ? (
        <p className="text-sm text-zinc-500">No posts yet.</p>
      ) : (
        <ul className="divide-y divide-zinc-100 dark:divide-zinc-800">
          {posts.map((post) => (
            <li key={post.id} className="py-3 flex items-center justify-between">
              <div>
                <Link
                  href={`/posts/${post.id}`}
                  className="text-sm font-medium hover:underline"
                >
                  {post.title}
                </Link>
                <p className="text-xs text-zinc-500 mt-0.5">{post.slug}</p>
              </div>
              <span className="text-xs text-zinc-400">{post.status}</span>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
