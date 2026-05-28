import type { Post, Project } from "./types";

const baseUrl = process.env.API_BASE_URL!;
const apiKey = process.env.API_KEY!;

function headers(): HeadersInit {
  return { "Content-Type": "application/json", "X-Api-Key": apiKey };
}

async function get<T>(path: string): Promise<T> {
  const res = await fetch(`${baseUrl}${path}`, { headers: headers(), cache: "no-store" });
  if (!res.ok) throw new Error(`GET ${path} failed: ${res.status}`);
  return res.json();
}

async function post<T>(path: string, body: unknown): Promise<T> {
  const res = await fetch(`${baseUrl}${path}`, {
    method: "POST",
    headers: headers(),
    body: JSON.stringify(body),
  });
  if (!res.ok) throw new Error(`POST ${path} failed: ${res.status}`);
  return res.json();
}

async function put(path: string, body: unknown): Promise<void> {
  const res = await fetch(`${baseUrl}${path}`, {
    method: "PUT",
    headers: headers(),
    body: JSON.stringify(body),
  });
  if (!res.ok) throw new Error(`PUT ${path} failed: ${res.status}`);
}

async function postEmpty(path: string): Promise<void> {
  const res = await fetch(`${baseUrl}${path}`, { method: "POST", headers: headers() });
  if (!res.ok) throw new Error(`POST ${path} failed: ${res.status}`);
}

export const api = {
  posts: {
    list: () => get<Post[]>("/api/posts"),
    get: (slug: string) => get<Post>(`/api/posts/${slug}`),
    create: (body: { title: string; slug: string; summary: string }) =>
      post<Post>("/api/posts", body),
    update: (
      id: string,
      body: { title: string; summary: string; content: string | null; tags: string[] }
    ) => put(`/api/posts/${id}`, body),
    publish: (id: string) => postEmpty(`/api/posts/${id}/publish`),
    archive: (id: string) => postEmpty(`/api/posts/${id}/archive`),
  },
  projects: {
    list: () => get<Project[]>("/api/projects"),
    get: (slug: string) => get<Project>(`/api/projects/${slug}`),
    create: (body: { title: string; slug: string; summary: string }) =>
      post<Project>("/api/projects", body),
    update: (
      id: string,
      body: {
        title: string;
        summary: string;
        content: string | null;
        coverImageUrl: string | null;
        techStack: string[];
        links: { label: string; url: string }[];
        displayOrder: number;
      }
    ) => put(`/api/projects/${id}`, body),
    publish: (id: string) => postEmpty(`/api/projects/${id}/publish`),
    archive: (id: string) => postEmpty(`/api/projects/${id}/archive`),
  },
};
