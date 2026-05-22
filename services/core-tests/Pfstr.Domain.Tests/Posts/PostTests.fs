module Pfstr.Domain.Tests.Posts.PostTests

open System
open Xunit
open Pfstr.Domain
open Pfstr.Domain.Posts

let private unwrapOk = function Ok v -> v | Error e -> failwith e

let private slug = Slug.create "my-first-post" |> unwrapOk
let private now = DateTimeOffset.Parse("2026-01-01T00:00:00Z")
let private id = PostId(Guid.NewGuid())

let private newPost () =
    Post.create id "My First Post" slug "A summary" now

[<Fact>]
let ``create returns post with Draft status`` () =
    Assert.Equal(Draft, (newPost ()).Status)

[<Fact>]
let ``create sets CreatedAt from provided timestamp`` () =
    Assert.Equal(now, (newPost ()).CreatedAt)

[<Fact>]
let ``create sets PublishedAt to None`` () =
    Assert.True((newPost ()).PublishedAt.IsNone)

[<Fact>]
let ``create sets Content to None`` () =
    Assert.True((newPost ()).Content.IsNone)

[<Fact>]
let ``create sets Tags to empty list`` () =
    Assert.Empty((newPost ()).Tags)

[<Fact>]
let ``publish on Draft returns Published with PublishedAt set`` () =
    let publishedAt = now.AddDays(1.0)
    match Post.publish publishedAt (newPost ()) with
    | Ok p ->
        Assert.Equal(Published, p.Status)
        Assert.Equal(Some publishedAt, p.PublishedAt)
    | Error e -> failwith e

[<Fact>]
let ``publish on Published post returns Error`` () =
    let published = Post.publish now (newPost ()) |> unwrapOk
    Assert.True(Result.isError (Post.publish now published))

[<Fact>]
let ``publish on Archived post returns Error`` () =
    let archived = Post.archive (newPost ()) |> unwrapOk
    Assert.True(Result.isError (Post.publish now archived))

[<Fact>]
let ``archive on Draft post returns Archived`` () =
    match Post.archive (newPost ()) with
    | Ok p -> Assert.Equal(Archived, p.Status)
    | Error e -> failwith e

[<Fact>]
let ``archive on Published post returns Archived`` () =
    let published = Post.publish now (newPost ()) |> unwrapOk
    match Post.archive published with
    | Ok p -> Assert.Equal(Archived, p.Status)
    | Error e -> failwith e

[<Fact>]
let ``archive on already Archived post returns Error`` () =
    let archived = Post.archive (newPost ()) |> unwrapOk
    Assert.True(Result.isError (Post.archive archived))

[<Fact>]
let ``update changes title summary content and tags`` () =
    let updated = Post.update "New Title" "New Summary" (Some "Body content") ["fsharp"; "dotnet"] (newPost ())
    Assert.Equal("New Title", updated.Title)
    Assert.Equal("New Summary", updated.Summary)
    Assert.Equal(Some "Body content", updated.Content)
    Assert.Equal<string list>(["fsharp"; "dotnet"], updated.Tags)

[<Fact>]
let ``update preserves id slug status and timestamps`` () =
    let p = newPost ()
    let updated = Post.update "New Title" "New Summary" None [] p
    Assert.Equal(p.Id, updated.Id)
    Assert.Equal(Slug.value p.Slug, Slug.value updated.Slug)
    Assert.Equal(p.Status, updated.Status)
    Assert.Equal(p.CreatedAt, updated.CreatedAt)
