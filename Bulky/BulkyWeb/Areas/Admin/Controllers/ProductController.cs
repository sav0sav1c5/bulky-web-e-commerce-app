using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            // Getting all 'Product' objects from Database
            List<Product> objProducts = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            return View(objProducts);
        }

        public IActionResult Upsert(int? id)
        {

            ProductViewModel productViewModel = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };

            // Id is not present - Create
            if (id == 0 || id == null)
            {
                return View(productViewModel);
            }
            // Id is present - Update
            else
            {
                productViewModel.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productViewModel);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductViewModel productViewModel, IFormFile? file)
        {
            // ModelState.IsValid - Check if model state (in this case 'Product') is valid
            // It goes in model of entity and checks all validations
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    // Update method addition - Checking if file (imageURL is present)
                    //if (productViewModel.Product.ImageURL != null)
                    if (!string.IsNullOrEmpty(productViewModel.Product.ImageURL))
                    {
                        // If it's not NULL or Empty that imageURL exists
                        // So we need to delete the old image and upload new
                        // In imageURL we have \images\product\fileName and because we
                        // use Path.Combine, we need to remove that part
                        var oldImagePath = Path.Combine(wwwRootPath, productViewModel.Product.ImageURL.TrimStart('\\'));

                        // Deleting old image
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }

                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productViewModel.Product.ImageURL = @"\images\product\" + fileName;
                }

                // If Id is present, we are updating Product
                if (productViewModel.Product.Id != 0)
                {
                    _unitOfWork.Product.Update(productViewModel.Product);
                }
                else
                {
                    _unitOfWork.Product.Add(productViewModel.Product);
                }
                
                _unitOfWork.Save();
                TempData["success"] = "Product created successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                // If model state is not valid, it expects dropdown entity list to be
                // populated, so we will just populate it in else block by adding
                // CategoryList to passed obj of 'ProductViewModel'
                productViewModel.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productViewModel);
            }
        }

        // Region for API calls
        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToDelete = _unitOfWork.Product.Get(u => u.Id == id);

            // If product isn't found we return error message
            if (productToDelete == null)
            {
                return Json(new { success = false, message = "Error while deleting product."});
            }

            // If product is found we remove it
            // But prior to that we need to check if product has imageURL -> image in wwwroot/images/product
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToDelete.
                ImageURL.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Product.Remove(productToDelete);
            _unitOfWork.Save();

            return Json(new { success = true, message =  "Product deleted successcully!"});
        }

        #endregion
    }
}
