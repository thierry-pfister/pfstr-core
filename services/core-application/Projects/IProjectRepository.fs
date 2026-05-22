namespace Pfstr.Application.Projects

open System.Threading.Tasks
open Pfstr.Domain.Projects

type ProjectApplicationError =
    | ValidationError of string
    | NotFound of string
    | Conflict of string
    | DomainError of string

type IProjectRepository =
    abstract member FindById: ProjectId -> Task<Project option>
    abstract member FindBySlug: Slug.T -> Task<Project option>
    abstract member FindAll: unit -> Task<Project list>
    abstract member Save: Project -> Task
