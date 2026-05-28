"use server";

import { revalidatePath } from "next/cache";
import { redirect } from "next/navigation";
import { api } from "../api";

type ActionState = { error: string } | null;

export async function createProject(prevState: ActionState, formData: FormData): Promise<ActionState> {
  const title = formData.get("title") as string;
  const slug = formData.get("slug") as string;
  const summary = formData.get("summary") as string;

  let project: Awaited<ReturnType<typeof api.projects.create>>;
  try {
    project = await api.projects.create({ title, slug, summary });
  } catch {
    return { error: "Failed to create project. Is the API running?" };
  }

  revalidatePath("/projects");
  redirect(`/projects/${project.id}`);
}

export async function updateProject(id: string, formData: FormData): Promise<void> {
  const title = formData.get("title") as string;
  const summary = formData.get("summary") as string;
  const content = (formData.get("content") as string) || null;
  const coverImageUrl = (formData.get("coverImageUrl") as string) || null;
  const techStackRaw = (formData.get("techStack") as string) || "";
  const techStack = techStackRaw.split(",").map((t) => t.trim()).filter(Boolean);
  const displayOrder = parseInt(formData.get("displayOrder") as string, 10) || 0;

  await api.projects.update(id, { title, summary, content, coverImageUrl, techStack, links: [], displayOrder });
  revalidatePath("/projects");
  revalidatePath(`/projects/${id}`);
}

export async function publishProject(id: string): Promise<void> {
  await api.projects.publish(id);
  revalidatePath("/projects");
  revalidatePath(`/projects/${id}`);
}

export async function archiveProject(id: string): Promise<void> {
  await api.projects.archive(id);
  revalidatePath("/projects");
  revalidatePath(`/projects/${id}`);
}
