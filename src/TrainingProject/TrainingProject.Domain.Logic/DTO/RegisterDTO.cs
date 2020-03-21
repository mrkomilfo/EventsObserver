using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingProject.Domain.Logic.DTO
{
    public class RegisterDTO
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
