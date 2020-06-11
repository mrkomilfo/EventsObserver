using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using TrainingProject.DomainLogic.Helpers;

namespace TrainingProject.DomainLogic.Annotations
{
    public class FileType : ValidationAttribute
    {
        readonly List<string> _types;

        public FileType()
        {
        }

        public FileType(string types)
        {
            _types = types.ParseSubstrings(",").ToList();
        }
        public override bool IsValid(object value)
        {
            IFormFile file = (IFormFile)value;
            return file is null || _types.Any(ext=>Regex.IsMatch(file.FileName, @$".*\.{ext}$"));
        }
    }
}
