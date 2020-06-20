using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingProject.Domain;
using TrainingProject.DomainLogic.Models.Categories;

namespace TrainingProject.DomainLogic.Interfaces
{
    public interface ICategoryManager
    {
        Task AddCategoryAsync(CategoryCreateDTO category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(int categoryId, bool force);
        Task<CategoryFullDTO> GetCategoryAsync(int categoryId);
        Task<ICollection<CategoryLiteDTO>> GetCategoriesAsync();
    }
}
