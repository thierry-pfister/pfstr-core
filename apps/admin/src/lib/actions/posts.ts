"use server";

import { revalidatePath } from "next/cache";
import { redirect } from "next/navigation";
import { api } from "../api";

export async function createPost(formData: FormData) {
  const title = formData.get("title") as string;
  const slug = formData.get("slug") as string;
  const summary = formData.get("summary") as string;

  const post = await api.posts.create({ title, slug, summary });
  revalidatePath("/posts");
  redirect(`/posts/${post.id}`);
}

export async function updatePost(id: string, formData: FormData) {
  const title = formData.get("title") as string;
  const summary = formData.get("summary") as string;
  const content = (formData.get("content") as string) || null;
  const tagsRaw = (formData.get("tags") as string) || "";
  const tags = tagsRaw
    .split(",")
    .map((t) => t.trim())
    .filter(Boolean);

  await api.posts.update(id, { title, summary, content, tags });
  revalidatePath("/posts");
  revalidatePath(`/posts/${id}`);
}

export async function publishPost(id: string) {
  await api.posts.publish(id);
  revalidatePath("/posts");
  revalidatePath(`/posts/${id}`);
}

export async function archivePost(id: string) {
  await api.posts.archive(id);
  revalidatePath("/posts");
  revalidatePath(`/posts/${id}`);
}
