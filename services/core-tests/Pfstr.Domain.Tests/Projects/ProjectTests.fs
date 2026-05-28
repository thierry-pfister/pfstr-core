module Pfstr.Domain.Tests.Projects.ProjectTests

open System
open Xunit
open Pfstr.Domain
open Pfstr.Domain.Projects

let private unwrapOk = function Ok v -> v | Error e -> failwith e

let private slug = Slug.create "test-project" |> unwrapOk
let private now = DateTimeOffset.Parse("2026-01-01T00:00:00Z")
let private id = ProjectId(Guid.NewGuid())

let private newProject () =
    Project.create id "Test Project" slug "A test project" now

[<Fact>]
let ``create returns project with Draft status`` () =
    Assert.Equal(Draft, (newProject ()).Status)

[<Fact>]
let ``create sets CreatedAt from provided timestamp`` () =
    Assert.Equal(now, (newProject ()).CreatedAt)

[<Fact>]
let ``create sets PublishedAt to None`` () =
    Assert.True((newProject ()).PublishedAt.IsNone)

[<Fact>]
let ``publish on Draft returns Published with PublishedAt set`` () =
    let publishedAt = now.AddDays(1.0)
    match Project.publish publishedAt (newProject ()) with
    | Ok p ->
        Assert.Equal(Published, p.Status)
        Assert.Equal(Some publishedAt, p.PublishedAt)
    | Error e -> failwith e

[<Fact>]
let ``publish on Published project returns Error`` () =
    let published = Project.publish now (newProject ()) |> unwrapOk
    Assert.True(Result.isError (Project.publish now published))

[<Fact>]
let ``publish on Archived project returns Error`` () =
    let archived = Project.archive (newProject ()) |> unwrapOk
    Assert.True(Result.isError (Project.publish now archived))

[<Fact>]
let ``archive on Draft project returns Archived`` () =
    match Project.archive (newProject ()) with
    | Ok p -> Assert.Equal(Archived, p.Status)
    | Error e -> failwith e

[<Fact>]
let ``archive on Published project returns Archived`` () =
    let published = Project.publish now (newProject ()) |> unwrapOk
    match Project.archive published with
    | Ok p -> Assert.Equal(Archived, p.Status)
    | Error e -> failwith e

[<Fact>]
let ``archive on already Archived project returns Error`` () =
    let archived = Project.archive (newProject ()) |> unwrapOk
    Assert.True(Result.isError (Project.archive archived))

[<Fact>]
let ``update changes title summary content techstack links coverImageUrl and displayOrder`` () =
    let links = [ { Label = "GitHub"; Url = "https://github.com" } ]
    let updated = Project.update "New Title" "New Summary" (Some "Content") ["F#"] links (Some "https://example.com/img.png") 5 (newProject ())
    Assert.Equal("New Title", updated.Title)
    Assert.Equal("New Summary", updated.Summary)
    Assert.Equal(Some "Content", updated.Content)
    Assert.Equal<string list>(["F#"], updated.TechStack)
    Assert.Equal(Some "https://example.com/img.png", updated.CoverImageUrl)
    Assert.Equal(5, updated.DisplayOrder)

[<Fact>]
let ``update clears coverImageUrl when None is passed`` () =
    let withImage = Project.update "T" "S" None [] [] (Some "https://example.com/img.png") 0 (newProject ())
    let cleared = Project.update "T" "S" None [] [] None 0 withImage
    Assert.True(cleared.CoverImageUrl.IsNone)

[<Fact>]
let ``update preserves id slug status and timestamps`` () =
    let p = newProject ()
    let updated = Project.update "New Title" "New Summary" None [] [] None 0 p
    Assert.Equal(p.Id, updated.Id)
    Assert.Equal(Slug.value p.Slug, Slug.value updated.Slug)
    Assert.Equal(p.Status, updated.Status)
    Assert.Equal(p.CreatedAt, updated.CreatedAt)

[<Fact>]
let ``create sets CoverImageUrl to None`` () =
    Assert.True((newProject ()).CoverImageUrl.IsNone)
