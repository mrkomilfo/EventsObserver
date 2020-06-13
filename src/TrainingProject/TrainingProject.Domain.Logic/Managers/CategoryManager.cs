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
using TrainingProject.Common;

namespace TrainingProject.DomainLogic.Managers
{
    public class CategoryManager: ICategoryManager
    {
        private readonly IAppContext _appContext;
        private readonly IMapper _mapper;
        private readonly ILogHelper _logger;

        public CategoryManager(IAppContext appContext, IMapper mapper, ILogHelper logger)
        {
            _appContext = appContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task AddCategory(CategoryCreateDTO category)
        {
            _logger.LogMethodCallingWithObject(category);
            if (await _appContext.Categories.AnyAsync(c => string.Equals(c.Name.ToLower(), category.Name.ToLower())))
            {
                throw new ArgumentException("Category with this name already exist");
            }
            await _appContext.Categories.AddAsync(_mapper.Map<Category>(category));
            await _appContext.SaveChangesAsync(default);
        }

        public async Task UpdateCategory(Category category)
        {
            _logger.LogMethodCallingWithObject(category);
            var update = await _appContext.Categories.FirstOrDefaultAsync(c => c.Id == category.Id);
            if (update == null)
            {
                throw new KeyNotFoundException($"Category with id={category.Id} not found");
            }
            update.Name = category.Name;
            update.Description = category.Description;
            await _appContext.SaveChangesAsync(default);
        }

        public async Task DeleteCategory(int categoryId, bool force = false)
        {
            _logger.LogMethodCallingWithObject(new { categoryId, force});
            var category = await _appContext.Categories.IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == categoryId);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with id={categoryId} not found");
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
            _logger.LogMethodCallingWithObject(new { categoryId });
            var category = await _appContext.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with id={categoryId} not found");
            }
            return _mapper.Map<CategoryFullDTO>(category);
        }
        public async Task<ICollection<CategoryLiteDTO>> GetCategories()
        {
            _logger.LogMethodCalling();
            return await _appContext.Categories.Select(c => new CategoryLiteDTO{ Id=c.Id, Name = c.Name }).ToListAsync();
        }
    }
}
