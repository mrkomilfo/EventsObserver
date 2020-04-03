using System;
using System.ComponentModel.DataAnnotations;

namespace TrainingProject.DomainLogic.Annotations
{
    public class ValidGuid : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return Guid.TryParse(value.ToString(), out _);
        }
    }
}