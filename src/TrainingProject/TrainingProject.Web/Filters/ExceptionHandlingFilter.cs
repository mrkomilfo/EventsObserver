using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
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
            
            var status = HttpStatusCode.InternalServerError;
            var message = context.Exception.Message;
            var exceptionType = context.Exception.GetType();
            
            if (exceptionType == typeof(UnauthorizedAccessException) 
                || exceptionType == typeof(SecurityTokenException))
            {
                status = HttpStatusCode.Unauthorized;
            }
            else if (exceptionType == typeof(KeyNotFoundException) || exceptionType == typeof(ObjectNotFoundException))
            {
                status = HttpStatusCode.NotFound;
            }
            else if (exceptionType == typeof(ArgumentException) || exceptionType == typeof(FormatException))
            {
                status = HttpStatusCode.BadRequest;
            }
            else if (exceptionType == typeof(AccessViolationException))
            {
                status = HttpStatusCode.Forbidden;
            }
            else if (exceptionType == typeof(ArgumentOutOfRangeException))
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
