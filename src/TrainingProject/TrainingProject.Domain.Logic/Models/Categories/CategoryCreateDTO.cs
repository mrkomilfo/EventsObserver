﻿using System.ComponentModel.DataAnnotations;

namespace TrainingProject.DomainLogic.Models.Categories
{
    public class CategoryCreateDTO
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
