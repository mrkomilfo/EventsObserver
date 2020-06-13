using System;

namespace TrainingProject.Common
{
    public interface ILogHelper
    {
        public void LogInfo(string message);
        public void LogError(Exception ex);
        void LogMethodCalling();
        void LogMethodCallingWithObject(object obj, string hide = "");
    }
}
