using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingProject.Common
{
    public interface ILogHelper
    {
        void LogMethodCalling();
        void LogMethodCallingWithObject(object obj);
    }
}
