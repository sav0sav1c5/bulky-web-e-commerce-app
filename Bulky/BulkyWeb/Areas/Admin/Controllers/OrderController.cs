using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
// using Stripe;
// using Stripe.Checkout;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderViewModel OrderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderId)
        {
            OrderViewModel orderViewModel = new()
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
            };

            return View(orderViewModel);
        }

        // Add UpdateOrderDetail action - allows admins and employees to update order information
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult UpdateOrderDetail()
        {
            var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVM.OrderHeader.City;
            orderHeaderFromDb.State = OrderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;

            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            }

            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
            {
                // Note: This is a bug in the original code - should be TrackingNumber
                // orderHeaderFromDb.Carrier = OrderVM.OrderHeader.TrackingNumber;
                orderHeaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }

            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();

            TempData["Success"] = "Order Details Updated Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
        }

        //// Add StartProcessing action - updates order status to "In Process"
        //[HttpPost]
        //[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        //public IActionResult StartProcessing()
        //{
        //    _unitOfWork.OrderHeader.UpUpdateStatus(OrderVM.OrderHeader.Id, SD.StatusInProcess);
        //    _unitOfWork.Save();
        //    TempData["Success"] = "Order Details Updated Successfully.";
        //    return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        //}

        //// Add ShipOrder action - updates order status to "Shipped"
        //[HttpPost]
        //[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        //public IActionResult ShipOrder()
        //{
        //    var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
        //    orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
        //    orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
        //    orderHeader.OrderStatus = SD.StatusShipped;
        //    orderHeader.ShippingDate = DateTime.Now;

        //    if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
        //    {
        //        orderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
        //    }

        //    _unitOfWork.OrderHeader.Update(orderHeader);
        //    _unitOfWork.Save();
        //    TempData["Success"] = "Order Shipped Successfully.";
        //    return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        //}

        //// Add CancelOrder action - cancels order and processes refund if needed
        //[HttpPost]
        //[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        //public IActionResult CancelOrder()
        //{
        //    var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);

        //    if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
        //    {
        //        // For Stripe integration - Uncomment when implementing Stripe
        //        /*
        //        var options = new RefundCreateOptions
        //        {
        //            Reason = RefundReasons.RequestedByCustomer,
        //            PaymentIntent = orderHeader.PaymentIntentId
        //        };

        //        var service = new RefundService();
        //        Refund refund = service.Create(options);
        //        */

        //        _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
        //    }
        //    else
        //    {
        //        _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
        //    }

        //    _unitOfWork.Save();
        //    TempData["Success"] = "Order Cancelled Successfully.";
        //    return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        //}

        //// Add Details_PAY_NOW action - initiates payment process
        //[ActionName("Details")]
        //[HttpPost]
        //public IActionResult Details_PAY_NOW()
        //{
        //    OrderVM.OrderHeader = _unitOfWork.OrderHeader
        //        .Get(u => u.Id == OrderVM.OrderHeader.Id, includeProperties: "ApplicationUser");
        //    OrderVM.OrderDetail = _unitOfWork.OrderDetail
        //        .GetAll(u => u.OrderHeaderId == OrderVM.OrderHeader.Id, includeProperties: "Product");

        //    // Stripe integration - Uncomment when implementing Stripe
        //    /*
        //    // Stripe logic
        //    var domain = Request.Scheme + "://" + Request.Host.Value + "/";
        //    var options = new SessionCreateOptions
        //    {
        //        SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderId={OrderVM.OrderHeader.Id}",
        //        CancelUrl = domain + $"admin/order/details?orderId={OrderVM.OrderHeader.Id}",
        //        LineItems = new List<SessionLineItemOptions>(),
        //        Mode = "payment",
        //    };

        //    foreach (var item in OrderVM.OrderDetail)
        //    {
        //        var sessionLineItem = new SessionLineItemOptions
        //        {
        //            PriceData = new SessionLineItemPriceDataOptions
        //            {
        //                UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
        //                Currency = "usd",
        //                ProductData = new SessionLineItemPriceDataProductDataOptions
        //                {
        //                    Name = item.Product.Title
        //                }
        //            },
        //            Quantity = item.Count
        //        };
        //        options.LineItems.Add(sessionLineItem);
        //    }

        //    var service = new SessionService();
        //    Session session = service.Create(options);
        //    _unitOfWork.OrderHeader.UpdateStripePaymentID(OrderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
        //    _unitOfWork.Save();
        //    Response.Headers.Add("Location", session.Url);
        //    return new StatusCodeResult(303);
        //    */

        //    // Placeholder return until Stripe is implemented
        //    TempData["Error"] = "Payment processing not implemented yet";
        //    return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        //}

        //// Add PaymentConfirmation action - confirms payment was successful
        //public IActionResult PaymentConfirmation(int orderHeaderId)
        //{
        //    OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderHeaderId);
        //    if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
        //    {
        //        // This is an order by company

        //        // Stripe integration - Uncomment when implementing Stripe
        //        /*
        //        var service = new SessionService();
        //        Session session = service.Get(orderHeader.SessionId);

        //        if (session.PaymentStatus.ToLower() == "paid")
        //        {
        //            _unitOfWork.OrderHeader.UpdateStripePaymentID(orderHeaderId, session.Id, session.PaymentIntentId);
        //            _unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
        //            _unitOfWork.Save();
        //        }
        //        */
        //    }

        //    return View(orderHeaderId);
        //}

        #region

        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> objOrderHeaderList = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();

            switch (status)
            {
                case "pending":
                    objOrderHeaderList = objOrderHeaderList.Where(u => u.OrderStatus == SD.StatusPending);
                    break;
                case "inprocess":
                    objOrderHeaderList = objOrderHeaderList.Where(u => u.OrderStatus == SD.StatusInProccess);
                    break;
                case "completed":
                    objOrderHeaderList = objOrderHeaderList.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    objOrderHeaderList = objOrderHeaderList.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }

            return Json(new { data = objOrderHeaderList });
        }

        #endregion
    }
}
