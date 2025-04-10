using BulkyWebRazor.Data;
using BulkyWebRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor.Pages.Categories
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public Category Category { get; set; }

        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
            // Just renders form on page
        }

        public IActionResult OnPost()
        {
            // Checking if there is already a category with same 'Category Name' and 'Display Order'
            var exsistingCategory = _db.Categories.FirstOrDefault(u =>
                u.Name == Category.Name && u.DisplayOrder == Category.DisplayOrder);

            if (exsistingCategory != null)
            {
                ModelState.AddModelError("", "Category with same 'Category Name' and 'DisplayOrder' alrady exists.");
            }

            // Checking if user trying to create Category with same 'Category Name' and 'Display Order'
            if (Category.Name == Category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Category.Name", "The 'Display Order' cannot exactly match the 'Category Name'.");
            }

            // If ModelState isn't isValid we are redisplaying the form
            if(!ModelState.IsValid)
            {
                return Page();
            }

            _db.Categories.Add(Category);
            _db.SaveChanges();
            TempData["success"] = "Category created successfully!";
            return RedirectToPage("Index");
        }
    }
}
