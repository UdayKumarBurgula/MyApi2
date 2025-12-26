using MyApi.Infrastructure.Persistence;

namespace MyApi.Api.Middleware
{
    public class SampleMiddleware
    {
        private readonly RequestDelegate _next;

        public SampleMiddleware(RequestDelegate next) // ✅ constructor clean
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext db) // ✅ inject here
        {
            // db is valid and scoped per request
            Console.WriteLine(db.GetHashCode());
            await _next(context);
        }
    }

}
