namespace Pfstr.Domain.Posts

open System
open Pfstr.Domain

type PostId = PostId of Guid

type PostStatus =
    | Draft
    | Published
    | Archived

type Post = {
    Id: PostId
    Title: string
    Slug: Slug.T
    Summary: string
    Content: string option
    Status: PostStatus
    Tags: string list
    CreatedAt: DateTimeOffset
    PublishedAt: DateTimeOffset option
}

module Post =

    let create (id: PostId) (title: string) (slug: Slug.T) (summary: string) (now: DateTimeOffset) : Post =
        { Id = id
          Title = title
          Slug = slug
          Summary = summary
          Content = None
          Status = Draft
          Tags = []
          CreatedAt = now
          PublishedAt = None }

    let publish (now: DateTimeOffset) (post: Post) : Result<Post, string> =
        match post.Status with
        | Published -> Error "Post is already published"
        | Archived  -> Error "Cannot publish an archived post"
        | Draft     -> Ok { post with Status = Published; PublishedAt = Some now }

    let archive (post: Post) : Result<Post, string> =
        match post.Status with
        | Archived -> Error "Post is already archived"
        | _        -> Ok { post with Status = Archived }
