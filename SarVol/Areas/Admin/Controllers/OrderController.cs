using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SarVol.DataAccess.Repository.IRepository;
using SarVol.Models;
using SarVol.Models.ViewModels;
using SarVol.Utility;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SarVol.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        public IActionResult Index()
        {
            return View();
        }

  


        [BindProperty]
        public OrderDetailsViewModel OrderDetails { get; set; }

        public IActionResult Details(int id)
        {
            OrderDetails = new OrderDetailsViewModel()
            {
                Header = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id, includeProperties: "AppUser"),
                Details = _unitOfWork.OrderDetails.GetAll(i => i.OrderId == id, includeProperties: "Product")
            };
            return View(OrderDetails);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Details")]
        public IActionResult DetailsPost(string stripeToken)
        {
            OrderHeader header = _unitOfWork.OrderHeader.GetFirstOrDefault(i => i.Id == OrderDetails.Header.Id);
            if (stripeToken != null)
            {
                var options = new ChargeCreateOptions
                {
                    Amount = Convert.ToInt32(header.OrderTotal * 100),
                    Currency = "usd",
                    Description = "Order ID: " + header.Id,
                    Source = stripeToken
                };
                var service = new ChargeService();
                Charge charge = service.Create(options);

                if (charge.BalanceTransactionId == null)
                {
                    header.PaymentStatus = StaticDetails.PaymentStatusRejected;
                    header.OrderStatus = StaticDetails.StatusPending;
                }
                else
                {
                    header.TransactionId = charge.BalanceTransactionId;
                }
                if (charge.Status.ToLower() == "succeeded")
                {
                    header.PaymentStatus = StaticDetails.PaymentStatusApproved;
                    header.OrderStatus = StaticDetails.StatusApproved;
                    header.PaymentDate = DateTime.Now;
                }
                _unitOfWork.Save();

            }
            return RedirectToAction("Details", "Order", new { id = header.Id });
        }


        #region Api calls
        [HttpGet]
        public IActionResult GetOrders(string status)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<OrderHeader> orderHeaders;

            if(User.IsInRole(StaticDetails.Role_Admin)|| User.IsInRole(StaticDetails.Role_Employee))
            {
                orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "AppUser");
            }
            else
            {
                orderHeaders = _unitOfWork.OrderHeader.GetAll(i => i.AppUserId == claim.Value, includeProperties: "AppUser");
            }



            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(i => i.PaymentStatus == StaticDetails.PaymentStatusPending 
                    || i.PaymentStatus == StaticDetails.PaymentStatusRejected);
                    break;
                case "inprocess":
                    orderHeaders = orderHeaders.Where(i => i.OrderStatus == StaticDetails.StatusInProcess 
                    || i.OrderStatus == StaticDetails.StatusPending 
                    || i.OrderStatus == StaticDetails.StatusApproved);

                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(i => i.OrderStatus == StaticDetails.StatusShipped);
                    break;
                case "rejected":
                    orderHeaders = orderHeaders.Where(i => i.OrderStatus == StaticDetails.StatusCancelled
                      || i.OrderStatus == StaticDetails.StatusRefunded);
                    break;
                case "all":
                 
                    break;

            }





            return Json(new { data = orderHeaders });
        }
        #endregion

        [Authorize(Roles =StaticDetails.Role_Admin+","+StaticDetails.Role_Employee)]
        public IActionResult HandleOrder(int id)
        {
            OrderHeader order = _unitOfWork.OrderHeader.GetFirstOrDefault(i => i.Id == id);
            order.OrderStatus = StaticDetails.StatusInProcess;
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }



        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        public IActionResult Shipment()
        {
            OrderHeader order = _unitOfWork.OrderHeader.GetFirstOrDefault(i => i.Id == OrderDetails.Header.Id);

            order.TrackingNumber = OrderDetails.Header.TrackingNumber;
            order.Carrier = OrderDetails.Header.Carrier;
            order.OrderStatus = StaticDetails.StatusShipped;
            order.ShippingDate = DateTime.Now;
          
            
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        
     

        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        public IActionResult Cancel(int id)
        {
            OrderHeader order = _unitOfWork.OrderHeader.GetFirstOrDefault(i => i.Id == id);

            if(order.PaymentStatus==StaticDetails.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Amount = Convert.ToInt32(order.OrderTotal * 100),
                    Reason = RefundReasons.RequestedByCustomer,
                    Charge = order.TransactionId
                };
                var refundService = new RefundService();

                Refund refund = refundService.Create(options);

                order.OrderStatus = StaticDetails.StatusRefunded;
                order.PaymentStatus = StaticDetails.StatusRefunded;
            }
            else
            {
                order.OrderStatus = StaticDetails.StatusCancelled;
                order.PaymentStatus = StaticDetails.StatusCancelled;
            }

            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }
    }
}
