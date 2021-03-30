using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Builder
{
    public static class SmokeAndMirrorExtensions
    {
        public static IEndpointConventionBuilder MapGet(
            this IEndpointRouteBuilder endpoints,
            string pattern,
            Func<TodoDbContext, Task<List<Todo>>> action)
        {
            return endpoints.MapGet(pattern, (Delegate)action);
        }

        public static IEndpointConventionBuilder MapGet(
            this IEndpointRouteBuilder endpoints,
            string pattern,
            Func<int, TodoDbContext, Task<IResult>> action)
        {
            return endpoints.MapGet(pattern, (Delegate)action);
        }

        public static IEndpointConventionBuilder MapPost(
            this IEndpointRouteBuilder endpoints,
            string pattern,
            Func<Todo, TodoDbContext, Task<IResult>> action)
        {
            return endpoints.MapPost(pattern, (Delegate)action);
        }

        public static IEndpointConventionBuilder MapPost(
            this IEndpointRouteBuilder endpoints,
            string pattern,
            Func<int, Todo, TodoDbContext, Task<IResult>> action)
        {
            return endpoints.MapPost(pattern, (Delegate)action);
        }

        public static IEndpointConventionBuilder MapDelete(
            this IEndpointRouteBuilder endpoints,
            string pattern,
            Func<int, TodoDbContext, Task<IResult>> action)
        {
            return endpoints.MapDelete(pattern, (Delegate)action);
        }
    }
}
