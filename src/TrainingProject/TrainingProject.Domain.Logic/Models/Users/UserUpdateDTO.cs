﻿using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using TrainingProject.DomainLogic.Annotations;

namespace TrainingProject.DomainLogic.Models.Users
{
    public class UserUpdateDto
    {
        [Required]
        [ValidGuid]
        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }

        public string ContactPhone { get; set; }

        [Required]
        public bool HasPhoto { get; set; }

        [MaxFileSize(8 * 1024 * 1024)]
        public IFormFile Photo { get; set; }
    }
}
