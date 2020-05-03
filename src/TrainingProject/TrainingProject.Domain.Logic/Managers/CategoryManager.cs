﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TrainingProject.Data;
using TrainingProject.Domain;
using TrainingProject.DomainLogic.Interfaces;
using Microsoft.EntityFrameworkCore;
using TrainingProject.DomainLogic.Models.Categories;
using System;

namespace TrainingProject.DomainLogic.Managers
{
    public class CategoryManager: ICategoryManager
    {
        private readonly IAppContext _appContext;
        private readonly IMapper _mapper;

        public CategoryManager(IAppContext appContext, IMapper mapper)
        {
            _appContext = appContext;
            _mapper = mapper;
        }

        public async Task AddCategory(CategoryCreateDTO category)
        {
            if (await _appContext.Categories.AnyAsync(c => string.Equals(c.Name.ToLower(), category.Name.ToLower())))
            {
                throw new ArgumentException("Category with this name already exist");
            }
            await _appContext.Categories.AddAsync(_mapper.Map<Category>(category));
            await _appContext.SaveChangesAsync(default);
        }

        public async Task UpdateCategory(Category category)
        {
            var update = await _appContext.Categories.FirstOrDefaultAsync(c => c.Id == category.Id);
            if (update == null)
            {
                throw new NullReferenceException($"Category with id={category.Id} not found");
            }
            update.Name = category.Name;
            update.Description = category.Description;
            await _appContext.SaveChangesAsync(default);
        }

        public async Task DeleteCategory(int categoryId, bool force = false)
        {
            var category = await _appContext.Categories.IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == categoryId);
            if (category == null)
            {
                throw new NullReferenceException($"Category with id={categoryId} not found");
            }
            if (force)
            {
                _appContext.Categories.Remove(category);
            }
            else
            {
                category.IsDeleted = true;
            }
            await _appContext.SaveChangesAsync(default);
        }

        public async Task<CategoryFullDTO> GetCategory(int categoryId)
        {
            var category = await _appContext.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
            if (category == null)
            {
                throw new NullReferenceException($"Category with id={categoryId} not found");
            }
            return _mapper.Map<CategoryFullDTO>(category);
        }
        public async Task<ICollection<CategoryLiteDTO>> GetCategories()
        {
            return await _appContext.Categories.Select(c => new CategoryLiteDTO{ Id=c.Id, Name = c.Name }).ToListAsync();
        }
    }
}
