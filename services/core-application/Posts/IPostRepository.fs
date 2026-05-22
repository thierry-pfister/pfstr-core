namespace Pfstr.Application.Posts

open System.Threading.Tasks
open Pfstr.Domain
open Pfstr.Domain.Posts

type PostApplicationError =
    | ValidationError of string
    | NotFound of string
    | Conflict of string
    | DomainError of string

type IPostRepository =
    abstract member FindById: PostId -> Task<Post option>
    abstract member FindBySlug: Slug.T -> Task<Post option>
    abstract member FindAll: unit -> Task<Post list>
    abstract member Save: Post -> Task
