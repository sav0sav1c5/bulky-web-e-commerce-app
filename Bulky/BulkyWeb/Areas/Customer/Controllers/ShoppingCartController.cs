using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        // When User fill details and hit submit button ViewModel will be automatically be populated
        [BindProperty]
        public ShoppingCartViewModel ShoppingCartViewModel { get; set; }

        public ShoppingCartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartViewModel = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"),
                OrderHeader = new()
            };

            foreach(var cart in ShoppingCartViewModel.ShoppingCartList)
            {
                cart.Price = GetTotalOrderPrice(cart);
                ShoppingCartViewModel.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartViewModel);
        }

        public IActionResult Plus(int shoppingCartId)
        {
            var shoppingCartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == shoppingCartId);
            shoppingCartFromDb.Count += 1;
            _unitOfWork.ShoppingCart.Update(shoppingCartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int shoppingCartId)
        {
            var shoppingCartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == shoppingCartId, tracked: true);

            if (shoppingCartFromDb.Count <= 1)
            {
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == shoppingCartFromDb.ApplicationUserId).Count() - 1);
                _unitOfWork.ShoppingCart.Remove(shoppingCartFromDb);
            }
            else
            {
                shoppingCartFromDb.Count -= 1;
                _unitOfWork.ShoppingCart.Update(shoppingCartFromDb);
            }

            _unitOfWork.Save();
            
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int shoppingCartId)
        {
            // We will need to add tracked true so entity is tracked and can be removed successfully
            var shoppingCartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == shoppingCartId, tracked: true);
            // Reduced by 1 because item will be removed in next line
            HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == shoppingCartFromDb.ApplicationUserId).Count() - 1);
            _unitOfWork.ShoppingCart.Remove(shoppingCartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartViewModel = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"),
                OrderHeader = new()
            };

            // Retrieving User that is placing order
            ShoppingCartViewModel.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            // Loading information of User that is placing order to summary
            ShoppingCartViewModel.OrderHeader.Name = ShoppingCartViewModel.OrderHeader.ApplicationUser.Name;
            ShoppingCartViewModel.OrderHeader.StreetAddress = ShoppingCartViewModel.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartViewModel.OrderHeader.PostalCode = ShoppingCartViewModel.OrderHeader.ApplicationUser.PostalCode;
            ShoppingCartViewModel.OrderHeader.City = ShoppingCartViewModel.OrderHeader.ApplicationUser.City;
            ShoppingCartViewModel.OrderHeader.State = ShoppingCartViewModel.OrderHeader.ApplicationUser.State;
            ShoppingCartViewModel.OrderHeader.PhoneNumber = ShoppingCartViewModel.OrderHeader.ApplicationUser.PhoneNumber;

            foreach (var shoppingCart in ShoppingCartViewModel.ShoppingCartList)
            {
                shoppingCart.Price = GetTotalOrderPrice(shoppingCart);
                ShoppingCartViewModel.OrderHeader.OrderTotal += (shoppingCart.Price * shoppingCart.Count);
            }

            return View(ShoppingCartViewModel);
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPOST()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Don't need to create instance of ViewModel
            ShoppingCartViewModel.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product");

            // Set Order Date to be today's Date
            ShoppingCartViewModel.OrderHeader.OrderDate = DateTime.Now;

            // Set logged user Id to applicationsUserId
            ShoppingCartViewModel.OrderHeader.ApplicationUserId = userId;

            // Retrieving User that is placing order
            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
            // ! - This will cause error, we add ApplicationUser here, but ET Core also pull one that needs to be in OrderHeader
            // NEVER populate a navigation property when you trying to insert new record in ETF Core
            foreach (var shoppingCart in ShoppingCartViewModel.ShoppingCartList)
            {
                shoppingCart.Price = GetTotalOrderPrice(shoppingCart);
                ShoppingCartViewModel.OrderHeader.OrderTotal += (shoppingCart.Price * shoppingCart.Count);
            }

            // Using GetValueOrDefault() because CompanyId can be null
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                // Customer -> Regular payment
                ShoppingCartViewModel.OrderHeader.OrderStatus = SD.StatusPending;
                ShoppingCartViewModel.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            }
            else
            {
                // Company -> Delayed payment
                ShoppingCartViewModel.OrderHeader.OrderStatus = SD.StatusApproved;
                ShoppingCartViewModel.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
            }

            // Now create OrderHeader
            _unitOfWork.OrderHeader.Add(ShoppingCartViewModel.OrderHeader);
            _unitOfWork.Save();

            // Now we have to create OrderDetails
            foreach (var cart in ShoppingCartViewModel.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartViewModel.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };

                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                // One more thing - if customer placed order we need to capture payment
                // Here will be Stripe logic
            }

            return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartViewModel.OrderHeader.Id });
        }

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, includeProperties: "ApplicationUser");
            
            if(orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                // Logic if stripe used with session
                // var service = new SessionService();
                // Session session = service.Get(orderHeader.SessionId);

                // if (session.PaymentStatus.ToLower() == "paid") 
                //  {
                //  _unitOfWork.OrderHeader.UpdateStringPaymentID(id, session.Id, session.PaymentIntentId);
                //  _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved,SD.PaymentStatusApproved);
                //  _unitOfWork.Save();
                //  }

                HttpContext.Session.Clear();
            }

            return View(id);
        }

        private double GetTotalOrderPrice(ShoppingCart obj)
        {
            if (obj.Count <= 50)
            {
                return obj.Product.Price;
            }
            else
            {
                if (obj.Count <= 100)
                {
                    return obj.Product.Price50;
                }
                else
                {
                    return obj.Product.Price100;
                }
            }
        }
    }
}
