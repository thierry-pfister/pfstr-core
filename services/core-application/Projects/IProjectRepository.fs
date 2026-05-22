namespace Pfstr.Application.Projects

open Pfstr.Domain.Projects

type ProjectApplicationError =
    | ValidationError of string
    | NotFound of string
    | Conflict of string
    | DomainError of string

type IProjectRepository =
    abstract member FindById: ProjectId -> Async<Project option>
    abstract member FindBySlug: Slug.T -> Async<Project option>
    abstract member FindAll: unit -> Async<Project list>
    abstract member Save: Project -> Async<unit>
