import { notFound } from "next/navigation";
import { api } from "@/lib/api";
import PostForm from "@/components/post-form";

export default async function EditPostPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;

  const posts = await api.posts.list();
  const post = posts.find((p) => p.id === id);
  if (!post) notFound();

  return (
    <div>
      <div className="mb-6">
        <h1 className="text-xl font-semibold">{post.title}</h1>
        <p className="text-xs text-zinc-500 mt-1">{post.slug} · {post.status}</p>
      </div>
      <PostForm post={post} />
    </div>
  );
}
