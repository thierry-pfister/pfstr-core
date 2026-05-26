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
    Subtitle: string option
    Content: string option
    CoverImage: string option
    CanonicalUrl: string option
    ReadingMinutes: int option
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
          Subtitle = None
          Content = None
          CoverImage = None
          CanonicalUrl = None
          ReadingMinutes = None
          Status = Draft
          Tags = []
          CreatedAt = now
          PublishedAt = None }

    let private computeReadingMinutes (content: string option) =
        match content with
        | None -> None
        | Some text ->
            let words = text.Split([| ' '; '\n'; '\r'; '\t' |], StringSplitOptions.RemoveEmptyEntries).Length
            Some (max 1 (words / 200))

    let publish (now: DateTimeOffset) (post: Post) : Result<Post, string> =
        match post.Status with
        | Published -> Error "Post is already published"
        | Archived  -> Error "Cannot publish an archived post"
        | Draft     ->
            Ok { post with
                   Status = Published
                   PublishedAt = Some now
                   ReadingMinutes = computeReadingMinutes post.Content }

    let archive (post: Post) : Result<Post, string> =
        match post.Status with
        | Archived -> Error "Post is already archived"
        | _        -> Ok { post with Status = Archived }

    let update
            (title: string) (summary: string) (subtitle: string option)
            (content: string option) (coverImage: string option)
            (canonicalUrl: string option) (tags: string list) (post: Post) : Post =
        { post with
            Title = title; Summary = summary; Subtitle = subtitle
            Content = content; CoverImage = coverImage; CanonicalUrl = canonicalUrl
            Tags = tags }
