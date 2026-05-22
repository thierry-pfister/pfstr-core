import { createProject } from "@/lib/actions/projects";

export default function NewProjectPage() {
  return (
    <div className="max-w-xl">
      <h1 className="text-xl font-semibold mb-6">New project</h1>
      <form action={createProject} className="flex flex-col gap-4">
        <Field label="Title" name="title" required />
        <Field label="Slug" name="slug" required placeholder="my-project-slug" />
        <Field label="Summary" name="summary" required multiline />
        <button
          type="submit"
          className="self-start rounded bg-zinc-900 px-4 py-2 text-sm text-white hover:bg-zinc-700 dark:bg-zinc-100 dark:text-zinc-900"
        >
          Create
        </button>
      </form>
    </div>
  );
}

function Field({
  label,
  name,
  required,
  placeholder,
  multiline,
}: {
  label: string;
  name: string;
  required?: boolean;
  placeholder?: string;
  multiline?: boolean;
}) {
  const base =
    "w-full rounded border border-zinc-200 bg-white px-3 py-2 text-sm dark:border-zinc-700 dark:bg-zinc-900 focus:outline-none focus:ring-2 focus:ring-zinc-400";
  return (
    <div className="flex flex-col gap-1">
      <label className="text-sm font-medium" htmlFor={name}>
        {label}
      </label>
      {multiline ? (
        <textarea id={name} name={name} required={required} rows={3} className={base} />
      ) : (
        <input
          id={name}
          name={name}
          required={required}
          placeholder={placeholder}
          className={base}
        />
      )}
    </div>
  );
}
