using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category");
            return View(productList);
        }

        public IActionResult Details(int productId)
        {
            // To make possible using ShoppingCart object on View
            // We need to create ShoppingCart with Product and pass that to View
            ShoppingCart shoppingCart = new()
            {
                Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category"),
                Count = 1,
                ProductId = productId
            };

            return View(shoppingCart);
        }

        // POST Action method for details
        // Added [Authorized] - if someone is posting need to be logged in, we don't care for role
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart obj)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            obj.ApplicationUserId = userId;

            // Checking if ShoppingCart with same ProductId and UserId already exist
            // ! - When we retrieve something with ETF Core know and track it
            //     so if we comment out update, ETF Core will anyway update object
            // It can be harmful so when we retrieve something with Get it shouldn't track that entity
            // and that will be set in repository
            ShoppingCart shoppingCartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userId && u.ProductId == obj.ProductId);

            if (shoppingCartFromDb != null)
            {
                // Update shopping cart
                shoppingCartFromDb.Count += obj.Count;
                _unitOfWork.ShoppingCart.Update(shoppingCartFromDb);
            }
            else
            {
                // Add shopping cart
                _unitOfWork.ShoppingCart.Add(obj);
            }
            
            _unitOfWork.Save();
            TempData["success"] = "Shopping cart updated successfully!";
            
            // Instead of using return RedirectToAction("Index");
            // We can use nameof(Index) to avoid spelling mistake
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}