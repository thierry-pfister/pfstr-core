namespace Pfstr.Application.Projects

open Pfstr.Domain.Projects

module GetProject =

    type Query = { Slug: string }

    let handle (repo: IProjectRepository) (query: Query) : Async<Result<Project, ProjectApplicationError>> =
        async {
            match Slug.create query.Slug with
            | Error msg -> return Error (ValidationError msg)
            | Ok slug ->
                let! project = repo.FindBySlug slug
                match project with
                | None   -> return Error (NotFound $"Project with slug '{query.Slug}' not found")
                | Some p -> return Ok p
        }

module ListProjects =

    type Query = { Status: ProjectStatus option }

    let handle (repo: IProjectRepository) (query: Query) : Async<Project list> =
        async {
            let! all = repo.FindAll()
            return
                match query.Status with
                | None        -> all
                | Some status -> all |> List.filter (fun p -> p.Status = status)
        }
