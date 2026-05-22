module Pfstr.Domain.Tests.Projects.SlugTests

open Xunit
open Pfstr.Domain.Projects

[<Theory>]
[<InlineData("my-project")>]
[<InlineData("hello-world-123")>]
[<InlineData("a")>]
[<InlineData("abc-123")>]
[<InlineData("123-project")>]
let ``create accepts valid slug`` (value: string) =
    Assert.True(Result.isOk (Slug.create value))

[<Theory>]
[<InlineData("")>]
[<InlineData("My-Project")>]
[<InlineData("my project")>]
[<InlineData("my_project")>]
[<InlineData("-starts-with-hyphen")>]
[<InlineData("ends-with-hyphen-")>]
let ``create rejects invalid slug`` (value: string) =
    Assert.True(Result.isError (Slug.create value))

[<Fact>]
let ``value returns the underlying string`` () =
    match Slug.create "my-project" with
    | Ok slug -> Assert.Equal("my-project", Slug.value slug)
    | Error e -> failwith e
