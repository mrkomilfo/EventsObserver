using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Serilog;
using System;
using System.Linq;
using TrainingProject.Common.Helpers;

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
            var method = new StackTrace().GetFrame(1).GetMethod();
            _logger.Information($"Call method {method.DeclaringType.FullName}.{method.Name}");
        }

        public void LogMethodCallingWithObject(object obj, string hide = "")
        {
            StringBuilder log = new StringBuilder($"Call method " +
                $"{new StackTrace().GetFrame(1).GetMethod().DeclaringType.FullName}");
            IList<PropertyInfo> props = new List<PropertyInfo>(obj.GetType().GetProperties());
            IList<string> toHide = hide.ParseSubstrings(",").ToList();
            foreach (PropertyInfo prop in props)
            {
                log.Append($"\n\t{prop.Name}: {(toHide.Contains(prop.Name) ? new string('*', prop.GetValue(obj, null).ToString().Length) : prop.GetValue(obj, null) ?? "null")}");
            }
            _logger.Information(log.ToString());
        }
    }
}
