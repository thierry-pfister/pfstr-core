namespace Pfstr.Domain

module Slug =

    type T = private Slug of string

    let private isValid (s: string) =
        not (System.String.IsNullOrEmpty(s))
        && s.Length <= 100
        && s[0] <> '-'
        && s[s.Length - 1] <> '-'
        && s |> Seq.forall (fun c -> (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c = '-')

    let create (value: string) : Result<T, string> =
        if isValid value then Ok(Slug value)
        else Error $"'{value}' is not a valid slug"

    let value (Slug s) = s
