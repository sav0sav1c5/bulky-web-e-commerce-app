using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        // Instead of using ApplicationDbContext we will use ICategoryRepository
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            //var objCategoryList = _db.Categories.ToList();
            List<Category> objCategories = _unitOfWork.Category.GetAll().ToList();
            return View(objCategories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            // Checking if there is already a category with same 'Category Name' and 'Display Order'
            var exsistingCategoty = _unitOfWork.Category.GetAll().FirstOrDefault(u => u.Name == obj.Name && u.DisplayOrder == obj.DisplayOrder);

            if (exsistingCategoty != null)
            {
                ModelState.AddModelError("", "Category with same 'Category Name' and 'DisplayOrder' alrady exists.");
            }

            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The 'Display Order' cannot exactly match the 'Category Name'.");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully!";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            // Tri nacina za dobavljanje iz baze
            // #1
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
            // #2
            // Category? categoryFromDb_2 = _db.Categories.FirstOrDefault(u => u.Id == id);
            // #3 - Retko, najcesce ako treba vise operacija pre vracanja
            // Category? categoryFromDb_3 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();

            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category updated successfully!";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            // Tri nacina za dobavljanje iz baze
            // #1
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
            // #2
            // Category? categoryFromDb_2 = _db.Categories.FirstOrDefault(u => u.Id == id);
            // #3 - Retko, najcesce ako treba vise operacija pre vracanja
            // Category? categoryFromDb_3 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();

            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Category? obj = _unitOfWork.Category.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}
