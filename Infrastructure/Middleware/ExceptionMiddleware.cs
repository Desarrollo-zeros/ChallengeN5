
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
namespace Infrastructure.Middleware
{
    public class ExceptionMiddleware 
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        //private readonly IServiceProvider _serviceProvider;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger
            //IServiceProvider serviceProvider
        )
        {
            _next = next;
            _logger = logger;
            //_serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            //using var scope = _serviceProvider.CreateScope();
            
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                await HandleExceptionAsync(httpContext, ex);
            }
            finally
            {
                _logger.LogInformation($"end process");
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError; // default status code

            // set the status code based on the exception type
            if (exception is ArgumentNullException)
            {
                code = HttpStatusCode.BadRequest;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(new
            {
                StatusCode = code,
                error = exception.Message
            }.ToString()) ;
        }

        public class Response
        {
            public int Status { get; set; }

            public string Message { get; set; }

        }

    }
}
