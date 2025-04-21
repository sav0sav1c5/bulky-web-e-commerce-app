using Bulky.DataAccess.Repository.IRepository;
using Bulky.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyWeb.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        // 1. Get shopping carts of logged-in user from database
        
        private readonly IUnitOfWork _unitOfWork;
        
        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        } 

        // Method that handles backend func. for ShoppingCartViewComponent
        public async Task<IViewComponentResult> InvokeAsync() {
            // Get id of logged user with claims
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claimedUser = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claimedUser != null)
            {
                if (HttpContext.Session.GetInt32(SD.SessionCart) == null)
                {
                    // Session is loaded and we don't need to go in database
                    // return View(HttpContext.Session.GetInt32(SD.SessionCart));
                    HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claimedUser.Value).Count());
                }

                return View(HttpContext.Session.GetInt32(SD.SessionCart));
            }
            else
            {
                // Can remove this from Logout because we handle it here
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
