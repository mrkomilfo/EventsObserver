using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Serilog;
using System;

namespace TrainingProject.Common
{
    public class LogHelper : ILogHelper
    {
        private ILogger _logger;
        public LogHelper(ILogger logger) {
            _logger = logger;
        }

        public void LogInfo(string message)
        {
            _logger.Information(message);
        }

        public void LogError(Exception ex)
        {
            _logger.Error(ex, ex.Message);
        }

        public void LogMethodCalling()
        {
            _logger.Information($"Call method {new StackTrace().GetFrame(1).GetMethod().DeclaringType.FullName}");
        }

        public void LogMethodCallingWithObject(object obj)
        {
            StringBuilder log = new StringBuilder($"Call method " +
                $"{new StackTrace().GetFrame(1).GetMethod().DeclaringType.FullName}");
            IList<PropertyInfo> props = new List<PropertyInfo>(obj.GetType().GetProperties());
            foreach (PropertyInfo prop in props)
            {
                log.Append($"\n\t{prop.Name}: {prop.GetValue(obj, null) ?? "null"}");
            }
            _logger.Information(log.ToString());
        }
    }
}
