using EFENGSI_RAHMANTO_ZALUKHU.Interfaces;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace EFENGSI_RAHMANTO_ZALUKHU.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategory _category;
        public CategoryController (ICategory category)
        {
           _category = category;
        }

        public IActionResult Index()
        {
            var data =_category.GetListCategory();
            return View(data);
        }

        public IActionResult Edit(int id)
        {
            var data =_category.GetCategoryById(id);
            return View(data);
        }

        [HttpPost]
        public IActionResult Edit(CategoryDTO categoryDTO)
        {
            if (categoryDTO.Id == 0)
            {
                var data =_category.AddCategory(categoryDTO);
                if (data)
                {
                    return RedirectToAction(nameof(Index));
                }

            }
            else
            {
                var data =_category.EditCategory(categoryDTO);
                if (data)
                {
                    return RedirectToAction(nameof(Index));
                }

            }
            return View();

        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var data =_category.DeleteCategory(id);
            if (data)
            {
                return RedirectToAction(nameof(Index));
            }
            return BadRequest("Gagal menghapus supplier.");
        }

    }
}
