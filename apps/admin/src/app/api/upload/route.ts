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

  // pfstr-core builds the full browser-reachable URL using its Uploads:BaseUrl config.
  // The admin proxy just passes it through unchanged.
  const data = await res.json();
  return NextResponse.json(data);
}
