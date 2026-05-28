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
  // url is a path like /static/assets/uuid.png — prefix the API base so the
  // portfolio can load it as a fully-qualified URL
  const fullUrl = `${process.env.API_BASE_URL}${url}`;
  return NextResponse.json({ url: fullUrl });
}
