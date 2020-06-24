using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Net;
using TrainingProject.Common;

namespace TrainingProject.Web.Filters
{
    public class ExceptionHandlingFilter : Attribute, IExceptionFilter
    {
        private readonly ILogHelper _logger;

        public ExceptionHandlingFilter(ILogHelper logger)
        {
            _logger = logger;
        }
        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception);
            HttpStatusCode status = HttpStatusCode.InternalServerError;
            string message = context.Exception.Message;
            var exceptionType = context.Exception.GetType();
            if (exceptionType == typeof(UnauthorizedAccessException) || exceptionType == typeof(SecurityTokenException))
            {
                status = HttpStatusCode.Unauthorized;
            }
            else if (exceptionType == typeof(KeyNotFoundException))
            {
                status = HttpStatusCode.NoContent;
            }
            else if (exceptionType == typeof(ArgumentOutOfRangeException))
            {
                status = HttpStatusCode.InternalServerError;
            }
            else if (exceptionType == typeof(ArgumentException))
            {
                status = HttpStatusCode.Conflict;
            }
            context.Result = new JsonResult(message)
            {
                StatusCode = (int?)status
            };
            context.ExceptionHandled = true;
        }
    }
}
