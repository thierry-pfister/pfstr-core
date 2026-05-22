namespace Pfstr.Application.Posts

open System
open Pfstr.Domain
open Pfstr.Domain.Posts

module CreatePost =

    type Command = { Title: string; Slug: string; Summary: string }

    let handle (repo: IPostRepository) (now: DateTimeOffset) (cmd: Command) : Async<Result<PostId, PostApplicationError>> =
        async {
            match Slug.create cmd.Slug with
            | Error msg -> return Error (ValidationError msg)
            | Ok slug ->
                let! existing = repo.FindBySlug slug |> Async.AwaitTask
                match existing with
                | Some _ -> return Error (Conflict $"Slug '{cmd.Slug}' is already taken")
                | None ->
                    let id = PostId(Guid.NewGuid())
                    do! repo.Save(Post.create id cmd.Title slug cmd.Summary now) |> Async.AwaitTask
                    return Ok id
        }

module PublishPost =

    type Command = { PostId: Guid }

    let handle (repo: IPostRepository) (now: DateTimeOffset) (cmd: Command) : Async<Result<unit, PostApplicationError>> =
        async {
            let! post = repo.FindById(PostId cmd.PostId) |> Async.AwaitTask
            match post with
            | None -> return Error (NotFound $"Post {cmd.PostId} not found")
            | Some p ->
                match Post.publish now p with
                | Error msg    -> return Error (DomainError msg)
                | Ok published ->
                    do! repo.Save published |> Async.AwaitTask
                    return Ok ()
        }

module ArchivePost =

    type Command = { PostId: Guid }

    let handle (repo: IPostRepository) (now: DateTimeOffset) (cmd: Command) : Async<Result<unit, PostApplicationError>> =
        async {
            let! post = repo.FindById(PostId cmd.PostId) |> Async.AwaitTask
            match post with
            | None -> return Error (NotFound $"Post {cmd.PostId} not found")
            | Some p ->
                match Post.archive p with
                | Error msg   -> return Error (DomainError msg)
                | Ok archived ->
                    do! repo.Save archived |> Async.AwaitTask
                    return Ok ()
        }

module UpdatePost =

    type Command = {
        PostId: Guid
        Title: string
        Summary: string
        Content: string option
        Tags: string list
    }

    let handle (repo: IPostRepository) (cmd: Command) : Async<Result<unit, PostApplicationError>> =
        async {
            let! post = repo.FindById(PostId cmd.PostId) |> Async.AwaitTask
            match post with
            | None -> return Error (NotFound $"Post {cmd.PostId} not found")
            | Some p ->
                let updated = Post.update cmd.Title cmd.Summary cmd.Content cmd.Tags p
                do! repo.Save updated |> Async.AwaitTask
                return Ok ()
        }
