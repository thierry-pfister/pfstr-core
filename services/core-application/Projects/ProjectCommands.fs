namespace Pfstr.Application.Projects

open System
open Pfstr.Domain
open Pfstr.Domain.Projects

module CreateProject =

    type Command = { Title: string; Slug: string; Summary: string }

    let handle (repo: IProjectRepository) (now: DateTimeOffset) (cmd: Command) : Async<Result<ProjectId, ProjectApplicationError>> =
        async {
            match Slug.create cmd.Slug with
            | Error msg -> return Error (ValidationError msg)
            | Ok slug ->
                let! existing = repo.FindBySlug slug |> Async.AwaitTask
                match existing with
                | Some _ -> return Error (Conflict $"Slug '{cmd.Slug}' is already taken")
                | None ->
                    let id = ProjectId(Guid.NewGuid())
                    do! repo.Save(Project.create id cmd.Title slug cmd.Summary now) |> Async.AwaitTask
                    return Ok id
        }

module PublishProject =

    type Command = { ProjectId: Guid }

    let handle (repo: IProjectRepository) (now: DateTimeOffset) (cmd: Command) : Async<Result<unit, ProjectApplicationError>> =
        async {
            let! project = repo.FindById(ProjectId cmd.ProjectId) |> Async.AwaitTask
            match project with
            | None -> return Error (NotFound $"Project {cmd.ProjectId} not found")
            | Some p ->
                match Project.publish now p with
                | Error msg    -> return Error (DomainError msg)
                | Ok published ->
                    do! repo.Save published |> Async.AwaitTask
                    return Ok ()
        }

module ArchiveProject =

    type Command = { ProjectId: Guid }

    let handle (repo: IProjectRepository) (now: DateTimeOffset) (cmd: Command) : Async<Result<unit, ProjectApplicationError>> =
        async {
            let! project = repo.FindById(ProjectId cmd.ProjectId) |> Async.AwaitTask
            match project with
            | None -> return Error (NotFound $"Project {cmd.ProjectId} not found")
            | Some p ->
                match Project.archive p with
                | Error msg   -> return Error (DomainError msg)
                | Ok archived ->
                    do! repo.Save archived |> Async.AwaitTask
                    return Ok ()
        }

module UpdateProject =

    type Command = {
        ProjectId: Guid
        Title: string
        Summary: string
        Content: string option
        TechStack: string list
        Links: ProjectLink list
        DisplayOrder: int
    }

    let handle (repo: IProjectRepository) (cmd: Command) : Async<Result<unit, ProjectApplicationError>> =
        async {
            let! project = repo.FindById(ProjectId cmd.ProjectId) |> Async.AwaitTask
            match project with
            | None -> return Error (NotFound $"Project {cmd.ProjectId} not found")
            | Some p ->
                let updated = Project.update cmd.Title cmd.Summary cmd.Content cmd.TechStack cmd.Links cmd.DisplayOrder p
                do! repo.Save updated |> Async.AwaitTask
                return Ok ()
        }
