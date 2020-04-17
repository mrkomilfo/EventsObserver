using System;

namespace TrainingProject.DomainLogic.Exceptions
{
    public class EventException : Exception
    {
        public EventException(string message) : base(message)
        {
        }
    }
}
