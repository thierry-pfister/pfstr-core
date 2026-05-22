module Pfstr.Application.Tests.Posts.PostCommandTests

open System
open System.Collections.Generic
open System.Threading.Tasks
open Xunit
open Pfstr.Domain.Posts
open Pfstr.Application.Posts

let private now = DateTimeOffset.Parse("2026-01-01T00:00:00Z")

let private makeStub (initial: Post list) =
    let store = List<Post>(initial)
    { new IPostRepository with
        member _.FindById id =
            Task.FromResult(store |> Seq.tryFind (fun p -> p.Id = id))
        member _.FindBySlug slug =
            Task.FromResult(store |> Seq.tryFind (fun p -> p.Slug = slug))
        member _.FindAll() =
            Task.FromResult(store |> Seq.toList)
        member _.Save post =
            match store |> Seq.tryFindIndex (fun p -> p.Id = post.Id) with
            | Some i -> store.[i] <- post
            | None   -> store.Add(post)
            Task.CompletedTask }

let private unwrapOk = function Ok v -> v | Error e -> failwithf "Expected Ok but got: %A" e

let private createCmd : CreatePost.Command =
    { Title = "My First Post"; Slug = "my-first-post"; Summary = "A summary" }

[<Fact>]
let ``CreatePost returns PostId for valid command`` () =
    let result = CreatePost.handle (makeStub []) now createCmd |> Async.RunSynchronously
    Assert.True(Result.isOk result)

[<Fact>]
let ``CreatePost saves post as Draft`` () =
    let repo = makeStub []
    CreatePost.handle repo now createCmd |> Async.RunSynchronously |> ignore
    let posts = repo.FindAll().Result
    Assert.Single(posts) |> ignore
    Assert.Equal(Draft, posts[0].Status)

[<Fact>]
let ``CreatePost returns ValidationError for invalid slug`` () =
    let cmd = { createCmd with Slug = "My Post" }
    match CreatePost.handle (makeStub []) now cmd |> Async.RunSynchronously with
    | Error (ValidationError _) -> ()
    | other -> failwithf "Expected ValidationError, got %A" other

[<Fact>]
let ``CreatePost returns Conflict when slug already taken`` () =
    let repo = makeStub []
    CreatePost.handle repo now createCmd |> Async.RunSynchronously |> ignore
    match CreatePost.handle repo now createCmd |> Async.RunSynchronously with
    | Error (Conflict _) -> ()
    | other -> failwithf "Expected Conflict, got %A" other

[<Fact>]
let ``PublishPost transitions Draft to Published`` () =
    let repo = makeStub []
    let (PostId pid) = CreatePost.handle repo now createCmd |> Async.RunSynchronously |> unwrapOk
    let result = PublishPost.handle repo now { PostId = pid } |> Async.RunSynchronously
    Assert.True(Result.isOk result)
    let posts = repo.FindAll().Result
    Assert.Equal(Published, posts[0].Status)

[<Fact>]
let ``PublishPost returns NotFound for unknown post`` () =
    match PublishPost.handle (makeStub []) now { PostId = Guid.NewGuid() } |> Async.RunSynchronously with
    | Error (NotFound _) -> ()
    | other -> failwithf "Expected NotFound, got %A" other

[<Fact>]
let ``ArchivePost transitions post to Archived`` () =
    let repo = makeStub []
    let (PostId pid) = CreatePost.handle repo now createCmd |> Async.RunSynchronously |> unwrapOk
    let result = ArchivePost.handle repo now { PostId = pid } |> Async.RunSynchronously
    Assert.True(Result.isOk result)
    let posts = repo.FindAll().Result
    Assert.Equal(Archived, posts[0].Status)

[<Fact>]
let ``ArchivePost returns NotFound for unknown post`` () =
    match ArchivePost.handle (makeStub []) now { PostId = Guid.NewGuid() } |> Async.RunSynchronously with
    | Error (NotFound _) -> ()
    | other -> failwithf "Expected NotFound, got %A" other

let private updateCmd (pid: Guid) : UpdatePost.Command =
    { PostId = pid; Title = "Updated Title"; Summary = "Updated Summary"; Content = Some "Body"; Tags = ["fsharp"] }

[<Fact>]
let ``UpdatePost updates fields and saves`` () =
    let repo = makeStub []
    let (PostId pid) = CreatePost.handle repo now createCmd |> Async.RunSynchronously |> unwrapOk
    let result = UpdatePost.handle repo (updateCmd pid) |> Async.RunSynchronously
    Assert.True(Result.isOk result)
    let posts = repo.FindAll().Result
    Assert.Equal("Updated Title", posts[0].Title)
    Assert.Equal(Some "Body", posts[0].Content)

[<Fact>]
let ``UpdatePost returns NotFound for unknown post`` () =
    match UpdatePost.handle (makeStub []) (updateCmd (Guid.NewGuid())) |> Async.RunSynchronously with
    | Error (NotFound _) -> ()
    | other -> failwithf "Expected NotFound, got %A" other
