using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TrainingProject.DomainLogic.Annotations
{
    public class MaxFileSize : ValidationAttribute
    {
        readonly int _size;

        public MaxFileSize()
        {
        }

        public MaxFileSize(int size)
        {
            _size = size;
        }
        public override bool IsValid(object value)
        {
            IFormFile file = (IFormFile)value;
            return file is null || file.Length <= _size;
        }
    }
}
