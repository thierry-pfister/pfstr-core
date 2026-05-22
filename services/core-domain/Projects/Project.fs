namespace Pfstr.Domain.Projects

open System

type ProjectId = ProjectId of Guid

type ProjectStatus =
    | Draft
    | Published
    | Archived

type ProjectLink = { Label: string; Url: string }

type Project = {
    Id: ProjectId
    Title: string
    Slug: Slug.T
    Summary: string
    Content: string option
    Status: ProjectStatus
    TechStack: string list
    Links: ProjectLink list
    CreatedAt: DateTimeOffset
    PublishedAt: DateTimeOffset option
    DisplayOrder: int
}

module Project =

    let create (id: ProjectId) (title: string) (slug: Slug.T) (summary: string) (now: DateTimeOffset) : Project =
        { Id = id
          Title = title
          Slug = slug
          Summary = summary
          Content = None
          Status = Draft
          TechStack = []
          Links = []
          CreatedAt = now
          PublishedAt = None
          DisplayOrder = 0 }

    let publish (now: DateTimeOffset) (project: Project) : Result<Project, string> =
        match project.Status with
        | Published -> Error "Project is already published"
        | Archived  -> Error "Cannot publish an archived project"
        | Draft     -> Ok { project with Status = Published; PublishedAt = Some now }

    let archive (project: Project) : Result<Project, string> =
        match project.Status with
        | Archived -> Error "Project is already archived"
        | _        -> Ok { project with Status = Archived }
