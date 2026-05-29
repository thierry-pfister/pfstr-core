export type PostStatus = "Draft" | "Published" | "Archived";
export type ProjectStatus = "Draft" | "Published" | "Archived";

export type ProjectLink = {
  label: string;
  url: string;
};

export type Post = {
  id: string;
  title: string;
  slug: string;
  summary: string;
  content: string | null;
  status: PostStatus;
  tags: string[];
  createdAt: string;
  publishedAt: string | null;
};

export type Project = {
  id: string;
  title: string;
  slug: string;
  summary: string;
  content: string | null;
  coverImageUrl: string | null;
  status: ProjectStatus;
  techStack: string[];
  links: ProjectLink[];
  displayOrder: number;
  createdAt: string;
};
