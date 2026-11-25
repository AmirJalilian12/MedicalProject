using Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace Web.Middleware
{
    public class ExceptionMiddlewareHandler
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ExceptionMiddlewareHandler> _logger;
        private readonly RequestDelegate _next;

        public ExceptionMiddlewareHandler(
            IWebHostEnvironment env,
            ILogger<ExceptionMiddlewareHandler> logger,
            RequestDelegate next)
        {
            _env = env;
            _logger = logger;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                if (context.Response.HasStarted)
                {
                    _logger.LogError(
                        exception,
                        "Response already started; cannot write error.");

                    throw;
                }

                _logger.LogError(
                    exception,
                    "Unhandled exception at {Path}",
                    context.Request.Path);

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var result = HandleResult(
                    context,
                    exception,
                    options);

                await context.Response.WriteAsync(result);
            }
        }

        private string HandleResult(
            HttpContext context,
            Exception exception,
            JsonSerializerOptions options)
        {
            switch (exception)
            {
                case NotFoundException notFound:

                    context.Response.StatusCode =
                        (int)HttpStatusCode.NotFound;

                    return JsonSerializer.Serialize(
                        new ApiToReturn(
                            404,
                            notFound.Message,
                            notFound.Messages,
                            exception.Message),
                        options);

                case BadRequestException badRequest:

                    context.Response.StatusCode =
                        (int)HttpStatusCode.BadRequest;

                    return JsonSerializer.Serialize(
                        new ApiToReturn(
                            400,
                            badRequest.Message,
                            badRequest.Messages,
                            exception.Message),
                        options);

                case ValidationExceptionEntity validation:

                    context.Response.StatusCode =
                        (int)HttpStatusCode.BadRequest;

                    return JsonSerializer.Serialize(
                        new ApiToReturn(
                            400,
                            validation.Message,
                            validation.Messages,
                            exception.Message),
                        options);

                default:

                    context.Response.StatusCode =
                        (int)HttpStatusCode.InternalServerError;

                    var message = _env.IsDevelopment()
                        ? exception.Message
                        : "خطای داخلی سرور";

                    return JsonSerializer.Serialize(
                        new ApiToReturn(
                            500,
                            message),
                        options);
            }
        }
    }
}
