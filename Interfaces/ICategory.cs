using EFENGSI_RAHMANTO_ZALUKHU.Models.DB;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EFENGSI_RAHMANTO_ZALUKHU.Interfaces
{
    public interface ICategory
    {
        public List<CategoryDTO> GetListCategory();
        public Category GetCategoryById(int id);
        public bool EditCategory(CategoryDTO categoryDTO);
        public bool AddCategory(CategoryDTO categoryDTO);
        public bool DeleteCategory(int id);
        public List<SelectListItem> Categories();
    }
}
