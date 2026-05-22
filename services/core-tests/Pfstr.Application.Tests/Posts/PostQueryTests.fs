module Pfstr.Application.Tests.Posts.PostQueryTests

open System
open System.Collections.Generic
open System.Threading.Tasks
open Xunit
open Pfstr.Domain
open Pfstr.Domain.Posts
open Pfstr.Application.Posts

let private now = DateTimeOffset.Parse("2026-01-01T00:00:00Z")

let private makeSlug s = Slug.create s |> function Ok x -> x | Error e -> failwith e

let private makePost title slug =
    Post.create (PostId(Guid.NewGuid())) title (makeSlug slug) "Summary" now

let private makeStub (posts: Post list) =
    let store = List<Post>(posts)
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

[<Fact>]
let ``GetPost returns post for valid slug`` () =
    let post = makePost "My Post" "my-post"
    let repo = makeStub [post]
    match GetPost.handle repo { Slug = "my-post" } |> Async.RunSynchronously with
    | Ok p -> Assert.Equal(post.Id, p.Id)
    | Error e -> failwithf "Expected Ok, got %A" e

[<Fact>]
let ``GetPost returns NotFound for unknown slug`` () =
    match GetPost.handle (makeStub []) { Slug = "missing" } |> Async.RunSynchronously with
    | Error (NotFound _) -> ()
    | other -> failwithf "Expected NotFound, got %A" other

[<Fact>]
let ``GetPost returns ValidationError for invalid slug`` () =
    match GetPost.handle (makeStub []) { Slug = "INVALID SLUG" } |> Async.RunSynchronously with
    | Error (ValidationError _) -> ()
    | other -> failwithf "Expected ValidationError, got %A" other

[<Fact>]
let ``ListPosts with no filter returns all posts`` () =
    let posts = [ makePost "Post A" "post-a"; makePost "Post B" "post-b" ]
    let result = ListPosts.handle (makeStub posts) { Status = None } |> Async.RunSynchronously
    Assert.Equal(2, result.Length)

[<Fact>]
let ``ListPosts with status filter returns only matching posts`` () =
    let repo = makeStub []
    let draft = makePost "Draft" "draft-post"
    let published = Post.publish now (makePost "Published" "published-post") |> function Ok p -> p | Error e -> failwith e
    (repo.Save draft).Wait()
    (repo.Save published).Wait()
    let result = ListPosts.handle repo { Status = Some Published } |> Async.RunSynchronously
    Assert.Equal(1, result.Length)
    Assert.Equal(Published, result[0].Status)
