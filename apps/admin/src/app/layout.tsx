import type { Metadata } from "next";
import Nav from "@/components/nav";
import "./globals.css";

export const metadata: Metadata = {
  title: "pfstr admin",
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en" className="h-full">
      <body className="h-full flex bg-white dark:bg-zinc-950 text-zinc-900 dark:text-zinc-100">
        <Nav />
        <main className="flex-1 overflow-y-auto px-8 py-6">{children}</main>
      </body>
    </html>
  );
}
