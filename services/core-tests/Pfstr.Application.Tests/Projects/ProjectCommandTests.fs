module Pfstr.Application.Tests.Projects.ProjectCommandTests

open System
open System.Collections.Generic
open System.Threading.Tasks
open Xunit
open Pfstr.Domain.Projects
open Pfstr.Application.Projects

let private now = DateTimeOffset.Parse("2026-01-01T00:00:00Z")

let private makeStub (initial: Project list) =
    let store = List<Project>(initial)
    { new IProjectRepository with
        member _.FindById id =
            Task.FromResult(store |> Seq.tryFind (fun p -> p.Id = id))
        member _.FindBySlug slug =
            Task.FromResult(store |> Seq.tryFind (fun p -> p.Slug = slug))
        member _.FindAll() =
            Task.FromResult(store |> Seq.toList)
        member _.Save project =
            match store |> Seq.tryFindIndex (fun p -> p.Id = project.Id) with
            | Some i -> store.[i] <- project
            | None   -> store.Add(project)
            Task.CompletedTask }

let private unwrapOk = function Ok v -> v | Error e -> failwithf "Expected Ok but got: %A" e

let private createCmd : CreateProject.Command =
    { Title = "My Project"; Slug = "my-project"; Summary = "A project" }

[<Fact>]
let ``CreateProject returns ProjectId for valid command`` () =
    let result = CreateProject.handle (makeStub []) now createCmd |> Async.RunSynchronously
    Assert.True(Result.isOk result)

[<Fact>]
let ``CreateProject saves project as Draft`` () =
    let repo = makeStub []
    CreateProject.handle repo now createCmd |> Async.RunSynchronously |> ignore
    let projects = repo.FindAll().Result
    Assert.Single(projects) |> ignore
    Assert.Equal(Draft, projects[0].Status)

[<Fact>]
let ``CreateProject returns ValidationError for invalid slug`` () =
    let cmd = { createCmd with Slug = "My Project" }
    match CreateProject.handle (makeStub []) now cmd |> Async.RunSynchronously with
    | Error (ValidationError _) -> ()
    | other -> failwithf "Expected ValidationError, got %A" other

[<Fact>]
let ``CreateProject returns Conflict when slug already taken`` () =
    let repo = makeStub []
    CreateProject.handle repo now createCmd |> Async.RunSynchronously |> ignore
    match CreateProject.handle repo now createCmd |> Async.RunSynchronously with
    | Error (Conflict _) -> ()
    | other -> failwithf "Expected Conflict, got %A" other

[<Fact>]
let ``PublishProject transitions Draft to Published`` () =
    let repo = makeStub []
    let (ProjectId pid) = CreateProject.handle repo now createCmd |> Async.RunSynchronously |> unwrapOk
    let result = PublishProject.handle repo now { ProjectId = pid } |> Async.RunSynchronously
    Assert.True(Result.isOk result)
    let projects = repo.FindAll().Result
    Assert.Equal(Published, projects[0].Status)

[<Fact>]
let ``PublishProject returns NotFound for unknown project`` () =
    match PublishProject.handle (makeStub []) now { ProjectId = Guid.NewGuid() } |> Async.RunSynchronously with
    | Error (NotFound _) -> ()
    | other -> failwithf "Expected NotFound, got %A" other

[<Fact>]
let ``ArchiveProject transitions project to Archived`` () =
    let repo = makeStub []
    let (ProjectId pid) = CreateProject.handle repo now createCmd |> Async.RunSynchronously |> unwrapOk
    let result = ArchiveProject.handle repo now { ProjectId = pid } |> Async.RunSynchronously
    Assert.True(Result.isOk result)
    let projects = repo.FindAll().Result
    Assert.Equal(Archived, projects[0].Status)

[<Fact>]
let ``ArchiveProject returns NotFound for unknown project`` () =
    match ArchiveProject.handle (makeStub []) now { ProjectId = Guid.NewGuid() } |> Async.RunSynchronously with
    | Error (NotFound _) -> ()
    | other -> failwithf "Expected NotFound, got %A" other
