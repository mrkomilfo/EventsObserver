using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingProject.Domain;
using TrainingProject.DomainLogic.Models.Categories;
using TrainingProject.DomainLogic.Models.Events;

namespace TrainingProject.DomainLogic.Interfaces
{
    public interface ICategoryManager
    {
        Task<bool> AddCategory(CategoryCreateDTO category);
        Task UpdateCategory(Category category);
        Task DeleteCategory(int categoryId, bool force);
        Task<ICollection<CategoryLiteDTO>> GetCategories();
    }
}
