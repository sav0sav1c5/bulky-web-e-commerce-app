using Bulky.DataAccess.Data;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            //var objCategoryList = _db.Categories.ToList();
            List<Category> objCategories = _db.Categories.ToList();
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
            var exsistingCategoty = _db.Categories.FirstOrDefault(u => u.Name == obj.Name && u.DisplayOrder == obj.DisplayOrder );
            
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
                _db.Categories.Add(obj);
                _db.SaveChanges();
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
            Category? categoryFromDb = _db.Categories.Find(id);
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
                _db.Categories.Update(obj);
                _db.SaveChanges();
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
            Category? categoryFromDb = _db.Categories.Find(id);
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
            Category? obj = _db.Categories.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Categories.Remove(obj);
            _db.SaveChanges();
            TempData["success"] = "Category deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}
