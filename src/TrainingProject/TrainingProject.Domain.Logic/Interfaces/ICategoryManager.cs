using CSharpFunctionalExtensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingProject.Domain;
using TrainingProject.DomainLogic.Models.Categories;

namespace TrainingProject.DomainLogic.Interfaces
{
    public interface ICategoryManager
    {
        Task<bool> AddCategory(CategoryCreateDTO category);
        Task UpdateCategory(Category category);
        Task DeleteCategory(int categoryId, bool force);
        Task<Maybe<CategoryFullDTO>> GetCategory(int categoryId);
        Task<ICollection<CategoryLiteDTO>> GetCategories();
    }
}
