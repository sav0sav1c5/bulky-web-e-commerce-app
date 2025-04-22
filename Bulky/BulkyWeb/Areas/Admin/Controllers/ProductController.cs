using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
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
                productViewModel.Product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "ProductImages");
                return View(productViewModel);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductViewModel productViewModel, List<IFormFile>? files)
        {
            // ModelState.IsValid - Check if model state (in this case 'Product') is valid
            // It goes in model of entity and checks all validations
            if (ModelState.IsValid)
            {

                // Moved here so when we can first get product id and use it for folder for uploaded images
                if (productViewModel.Product.Id != 0)
                {
                    _unitOfWork.Product.Update(productViewModel.Product);
                }
                else
                {
                    _unitOfWork.Product.Add(productViewModel.Product);
                }

                _unitOfWork.Save();

                string wwwRootPath = _webHostEnvironment.WebRootPath;

                if (files != null)
                {

                    foreach (IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\product-" + productViewModel.Product.Id;
                        string finalProductPath = Path.Combine(wwwRootPath, productPath);

                        // If already don't exist we are creating directory
                        if (!Directory.Exists(finalProductPath))
                        {
                            Directory.CreateDirectory(finalProductPath);
                        }

                        // Uploading files
                        using (var fileStream = new FileStream(Path.Combine(finalProductPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        // After uploading need to save obj to ProductImage table
                        ProductImage productImage = new()
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId = productViewModel.Product.Id,
                        };

                        if (productViewModel.Product.ProductImages == null)
                        {
                            productViewModel.Product.ProductImages = new List<ProductImage>();
                        }

                        productViewModel.Product.ProductImages.Add(productImage);
                        // _unitOfWork.ProductImage.Add(productImage);
                    }

                    _unitOfWork.Product.Update(productViewModel.Product);
                    _unitOfWork.Save();

                    // Update method addition - Checking if file (imageURL is present)
                    // if (productViewModel.Product.ImageURL != null)

                    //    if (!string.IsNullOrEmpty(productViewModel.Product.ImageURL))
                    //    {
                    //        // If it's not NULL or Empty that imageURL exists
                    //        // So we need to delete the old image and upload new
                    //        // In imageURL we have \images\product\fileName and because we
                    //        // use Path.Combine, we need to remove that part
                    //        var oldImagePath = Path.Combine(wwwRootPath, productViewModel.Product.ImageURL.TrimStart('\\'));

                    //        // Deleting old image
                    //        if (System.IO.File.Exists(oldImagePath))
                    //        {
                    //            System.IO.File.Delete(oldImagePath);
                    //        }

                    //    }

                    //    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    //    {
                    //        file.CopyTo(fileStream);
                    //    }

                    //    productViewModel.Product.ImageURL = @"\images\product\" + fileName;
                }

                //// If Id is present, we are updating Product
                //if (productViewModel.Product.Id != 0)
                //{
                //    _unitOfWork.Product.Update(productViewModel.Product);
                //}
                //else
                //{
                //    _unitOfWork.Product.Add(productViewModel.Product);
                //}
                
                //_unitOfWork.Save();
                TempData["success"] = "Product created/updated successfully!";
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

        public IActionResult DeleteImage(int? imageId)
        {
            // We will get image that needs to be deleted
            var imageToBeDeleted = _unitOfWork.ProductImage.Get(u => u.Id == imageId);
            int productId = imageToBeDeleted.ProductId;

            // If it's not null, we also need to check URL
            if (imageToBeDeleted !=  null)
            {
                if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
                {
                    var imageToBeDeletedPath = Path.Combine(_webHostEnvironment.WebRootPath, imageToBeDeleted.ImageUrl.Trim('\\'));

                    if (System.IO.File.Exists(imageToBeDeletedPath))
                    {
                        System.IO.File.Delete(imageToBeDeletedPath);
                    }
                }

                // Also we need to remove that image from database
                _unitOfWork.ProductImage.Remove(imageToBeDeleted);
                _unitOfWork.Save();

                TempData["success"] = "Product image deleted successfully!";
            }

            return RedirectToAction(nameof(Upsert), new {id = productId});
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
            //var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToDelete.
            //    ImageURL.TrimStart('\\'));

            //if (System.IO.File.Exists(oldImagePath))
            //{
            //    System.IO.File.Delete(oldImagePath);
            //}

            string productPath = @"images\products\product-" + id;
            string finalProductPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

            // Deleting directory with images of product that is going to be deleted
            if (!Directory.Exists(finalProductPath))
            {
                // First we need to get all files and remove each of them
                string[] filePaths = Directory.GetFiles(finalProductPath);
                
                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }

                Directory.Delete(finalProductPath);
            }

            _unitOfWork.Product.Remove(productToDelete);
            _unitOfWork.Save();

            return Json(new { success = true, message =  "Product deleted successcully!"});
        }

        #endregion
    }
}
