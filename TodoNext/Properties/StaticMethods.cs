using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Demo
{
    public static class StaticMethods
    {
        public static IResult Ok(object value) => new JsonResult(value);
        public static IResult NoContent() => new NoContentResult();
        public static IResult NotFound() => new NotFoundResult();
    }
}
