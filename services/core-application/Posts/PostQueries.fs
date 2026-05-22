namespace Pfstr.Application.Posts

open Pfstr.Domain
open Pfstr.Domain.Posts

module GetPost =

    type Query = { Slug: string }

    let handle (repo: IPostRepository) (query: Query) : Async<Result<Post, PostApplicationError>> =
        async {
            match Slug.create query.Slug with
            | Error msg -> return Error (ValidationError msg)
            | Ok slug ->
                let! post = repo.FindBySlug slug |> Async.AwaitTask
                match post with
                | None   -> return Error (NotFound $"Post with slug '{query.Slug}' not found")
                | Some p -> return Ok p
        }

module ListPosts =

    type Query = { Status: PostStatus option }

    let handle (repo: IPostRepository) (query: Query) : Async<Post list> =
        async {
            let! all = repo.FindAll() |> Async.AwaitTask
            return
                match query.Status with
                | None        -> all
                | Some status -> all |> List.filter (fun p -> p.Status = status)
        }
