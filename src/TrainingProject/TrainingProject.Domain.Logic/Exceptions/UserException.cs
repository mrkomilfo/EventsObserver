using System;

namespace TrainingProject.DomainLogic.Exceptions
{
    public class UserException : Exception
    {
        public UserException(string message) : base(message)
        {
        }
    }
}
