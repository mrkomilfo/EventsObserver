using System;

namespace TrainingProject.DomainLogic.Exceptions
{
    public class CategoryException : Exception
    {
        public CategoryException(string message) : base(message)
        { 
        }
    }
}
