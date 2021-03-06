﻿using System.ComponentModel.DataAnnotations;
using TrainingProject.DomainLogic.Annotations;

namespace TrainingProject.DomainLogic.Models.Users
{
    public class ChangeRoleDto
    {
        [Required]
        [ValidGuid]
        public string UserId { get; set; }
        [Required]
        public int RoleId { get; set; }
    }
}
