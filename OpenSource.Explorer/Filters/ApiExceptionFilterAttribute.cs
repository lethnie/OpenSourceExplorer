using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Net;

namespace OpenSource.Explorer.Filters
{
    /// <summary>
    /// Filter that runs asynchronously after an action has thrown an <see cref="System.Exception"/>.
    /// </summary>
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Initialization.
        /// </summary>
        /// <param name="loggerFactory">Factory for log service.</param>
        public ApiExceptionFilterAttribute(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("InternalServerError");
        }

        /// <summary>
        /// Runs asynchronously after an action has thrown an <see cref="System.Exception"/>.
        /// </summary>
        /// <param name="context">Current exception context.</param>
        public override void OnException(ExceptionContext context)
        {
            string detail = null;
            if (context.Exception.Data.Contains("Details"))
            {
                detail = context.Exception.Data["Details"]?.ToString();
            }
            ApiError apiError = new ApiError()
            {
                Message = GetErrorMessage(context.Exception),
#if DEBUG
                Detail = detail ?? context.Exception.StackTrace
#else
                Detail = detail
#endif
            };

            _logger.LogError(context.Exception, context.Exception.Message);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Result = new JsonResult(apiError);

            base.OnException(context);
        }

        private string GetErrorMessage(Exception exception)
        {
            // TODO: custom error messages for different errors
            return "An error occurred while running the application. Please try again later.";
        }
    }
}
