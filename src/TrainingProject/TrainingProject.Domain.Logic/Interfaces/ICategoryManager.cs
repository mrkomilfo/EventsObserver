using System.Collections.Generic;
using System.Threading.Tasks;

using TrainingProject.Domain;
using TrainingProject.DomainLogic.Models.Categories;

namespace TrainingProject.DomainLogic.Interfaces
{
    public interface ICategoryManager
    {
        Task AddCategoryAsync(CategoryCreateDto category);

        Task UpdateCategoryAsync(Category category);

        Task DeleteCategoryAsync(int categoryId, bool force);

        Task<CategoryFullDto> GetCategoryAsync(int categoryId);

        ICollection<CategoryFullDto> GetCategories();

        ICollection<CategoryLiteDto> GetCategoryNames();
    }
}
