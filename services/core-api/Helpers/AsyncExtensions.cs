using Microsoft.FSharp.Control;
using Microsoft.FSharp.Core;

namespace Pfstr.Api.Helpers;

internal static class AsyncExtensions
{
    internal static Task<T> ToTask<T>(this FSharpAsync<T> async) =>
        FSharpAsync.StartAsTask(async, FSharpOption<TaskCreationOptions>.None, FSharpOption<CancellationToken>.None);
}
