module Pfstr.Application.Tests.Projects.ProjectQueryTests

open System
open System.Threading.Tasks
open Xunit
open Pfstr.Domain
open Pfstr.Domain.Projects
open Pfstr.Application.Projects

let private now = DateTimeOffset.Parse("2026-01-01T00:00:00Z")

let private makeSlug s = Slug.create s |> function Ok x -> x | Error e -> failwith e

let private makeProject title slug =
    Project.create (ProjectId(Guid.NewGuid())) title (makeSlug slug) "Summary" now

let private makeStub (projects: Project list) =
    { new IProjectRepository with
        member _.FindById id    = Task.FromResult(projects |> List.tryFind (fun p -> p.Id = id))
        member _.FindBySlug s   = Task.FromResult(projects |> List.tryFind (fun p -> p.Slug = s))
        member _.FindAll()      = Task.FromResult(projects)
        member _.Save _         = Task.CompletedTask }

[<Fact>]
let ``GetProject returns project for existing slug`` () =
    let project = makeProject "Test" "test-project"
    match GetProject.handle (makeStub [project]) { Slug = "test-project" } |> Async.RunSynchronously with
    | Ok p -> Assert.Equal(project.Id, p.Id)
    | Error e -> failwithf "Expected Ok, got %A" e

[<Fact>]
let ``GetProject returns NotFound for unknown slug`` () =
    match GetProject.handle (makeStub []) { Slug = "unknown" } |> Async.RunSynchronously with
    | Error (NotFound _) -> ()
    | other -> failwithf "Expected NotFound, got %A" other

[<Fact>]
let ``GetProject returns ValidationError for invalid slug`` () =
    match GetProject.handle (makeStub []) { Slug = "Invalid Slug" } |> Async.RunSynchronously with
    | Error (ValidationError _) -> ()
    | other -> failwithf "Expected ValidationError, got %A" other

[<Fact>]
let ``ListProjects returns all projects when no status filter`` () =
    let draft     = makeProject "Draft"     "draft-project"
    let published = makeProject "Published" "published-project" |> Project.publish now |> function Ok p -> p | Error e -> failwith e
    let result = ListProjects.handle (makeStub [draft; published]) { Status = None } |> Async.RunSynchronously
    Assert.Equal(2, result.Length)

[<Fact>]
let ``ListProjects filters by Published status`` () =
    let draft     = makeProject "Draft"     "draft-project"
    let published = makeProject "Published" "published-project" |> Project.publish now |> function Ok p -> p | Error e -> failwith e
    let result = ListProjects.handle (makeStub [draft; published]) { Status = Some Published } |> Async.RunSynchronously
    Assert.Equal(1, result.Length)
    Assert.Equal(Published, result[0].Status)
