import { NextRequest, NextResponse } from "next/server";

export async function POST(req: NextRequest) {
  const formData = await req.formData();

  const res = await fetch(`${process.env.API_BASE_URL}/api/assets`, {
    method: "POST",
    headers: { "X-Api-Key": process.env.API_KEY! },
    body: formData,
  });

  if (!res.ok) {
    const text = await res.text();
    return NextResponse.json({ error: text }, { status: res.status });
  }

  const { url } = (await res.json()) as { url: string };
  // PUBLIC_API_URL is the browser-reachable address (e.g. https://api.thierrypfister.dev).
  // API_BASE_URL is the internal Docker address used for server-to-server calls.
  // The stored URL must be browser-reachable so the portfolio can load it as a texture.
  const publicBase = process.env.PUBLIC_API_URL || process.env.API_BASE_URL;
  return NextResponse.json({ url: `${publicBase}${url}` });
}
