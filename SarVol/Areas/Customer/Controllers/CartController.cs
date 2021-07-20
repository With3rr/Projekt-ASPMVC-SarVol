using Microsoft.AspNetCore.Identity;
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

namespace SarVol.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public ShoppingCartViewModel ShoppingCartVM { get; set; }

        private readonly UserManager<IdentityUser> _userManager;

        public CartController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartViewModel shoppingCart = new ShoppingCartViewModel()
            {
                OrderHeader = new Models.OrderHeader(),
                Shoppings = _unitOfWork.ShoppingCart.GetAll(i => i.AppUserId == claim.Value, includeProperties: "Product")
                
            };
            shoppingCart.OrderHeader.OrderTotal = 0;

            shoppingCart.Count = shoppingCart.Shoppings.Count();


            shoppingCart.OrderHeader.AppUser = _unitOfWork.AppUser.GetFirstOrDefault(i => i.Id == claim.Value,includeProperties:"Company");

            foreach (var item in shoppingCart.Shoppings)
            {
                var whatPrice = item.Product.PromotionalPrice != null ? item.Product.PromotionalPrice : item.Product.RegularPrice;
        
                item.Price = (double)whatPrice;

                

                shoppingCart.OrderHeader.OrderTotal += item.Count * item.Price;
                item.Product.Description = RavHtmlConversion.ConvertToRawHtml(item.Product.Description);
                if(item.Product.Description.Length>100)
                {
                    item.Product.Description = item.Product.Description.Substring(0, 101)+"...";
                }
            }
            
            return View(shoppingCart);
        }

        public IActionResult More(int Id)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(i => i.Id == Id, includeProperties: "Product");

            cart.Count += 1;
            
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Less(int Id)
        {
           
           

            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(i => i.Id == Id, includeProperties: "Product");
            if(cart.Count==1)
            {
                _unitOfWork.ShoppingCart.Remove(Id);

                _unitOfWork.Save();

                var identity = (ClaimsIdentity)User.Identity;
                var claim = identity.FindFirst(ClaimTypes.NameIdentifier);
                var count = _unitOfWork.ShoppingCart.GetAll(i => i.AppUserId == claim.Value).ToList().Count();

                HttpContext.Session.SetObject(StaticDetails.SesionShoppingCart, count);
            }  
            else
            {
               
                cart.Count -= 1;
                _unitOfWork.Save();
               
            }
                
    
            

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int Id)
        {
           

            _unitOfWork.ShoppingCart.Remove(Id);
          
            _unitOfWork.Save();

            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);

            var count = _unitOfWork.ShoppingCart.GetAll(i => i.AppUserId == claim.Value).ToList().Count();

            HttpContext.Session.SetObject(StaticDetails.SesionShoppingCart, count);

            return RedirectToAction(nameof(Index));
        }


        public IActionResult Summary()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);
            var finalCart = new ShoppingCartViewModel()
            {
                OrderHeader = new Models.OrderHeader(),
                Shoppings = _unitOfWork.ShoppingCart.GetAll(i=>i.AppUserId==claim.Value,includeProperties:"Product")
            };
           

            finalCart.OrderHeader.AppUser = _unitOfWork.AppUser.GetFirstOrDefault(i=>i.Id==claim.Value,includeProperties:"Company");

            foreach (var item in finalCart.Shoppings)
            {
                var whatPrice = item.Product.PromotionalPrice != null ? item.Product.PromotionalPrice : item.Product.RegularPrice;
               
                item.Price = (double)whatPrice;

                item.FullPrice = item.Count * item.Price;
                finalCart.OrderHeader.OrderTotal += item.Count * item.Price;


            }
            finalCart.OrderHeader.Name = finalCart.OrderHeader.AppUser.Name;
            finalCart.OrderHeader.PhoneNumber = finalCart.OrderHeader.AppUser.PhoneNumber;
            finalCart.OrderHeader.Street = finalCart.OrderHeader.AppUser.Street;
            finalCart.OrderHeader.City = finalCart.OrderHeader.AppUser.City;
            finalCart.OrderHeader.Country = finalCart.OrderHeader.AppUser.Country;
            finalCart.OrderHeader.PostalCode = finalCart.OrderHeader.AppUser.PostalCode;

            return View(finalCart);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPost(string stripeToken)
        {
          
            
            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM.OrderHeader.AppUser = _unitOfWork.AppUser.GetFirstOrDefault(i => i.Id == claim.Value,includeProperties:"Company");

            ShoppingCartVM.Shoppings = _unitOfWork.ShoppingCart.GetAll(i => i.AppUserId == claim.Value,includeProperties:"Product");

            ShoppingCartVM.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusPending;
            ShoppingCartVM.OrderHeader.OrderStatus = StaticDetails.StatusPending;
            ShoppingCartVM.OrderHeader.AppUserId = claim.Value;
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.PaymentDueDate = DateTime.Now.AddDays(3);

            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

          
            foreach (var item in ShoppingCartVM.Shoppings)
            {
               

                var whatPrice = item.Product.PromotionalPrice != null ? item.Product.PromotionalPrice : item.Product.RegularPrice;
                var orderDetails = new OrderDetails()
                {
                    ProductId = item.Product.Id,
                    OrderId = ShoppingCartVM.OrderHeader.Id,
                    Price = item.Count * (double)whatPrice,
                    PricePerProduct =(double)whatPrice,
                    Count =item.Count

                };
                ShoppingCartVM.OrderHeader.OrderTotal += orderDetails.Price;
                _unitOfWork.OrderDetails.Add(orderDetails);
              
            }
            _unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVM.Shoppings);
            _unitOfWork.Save();
            HttpContext.Session.SetObject(StaticDetails.SesionShoppingCart, 0);

            if(stripeToken==null)
            {

            }
            else
            {
                var options = new ChargeCreateOptions
                {
                    Amount = Convert.ToInt32(ShoppingCartVM.OrderHeader.OrderTotal * 100),
                    Currency = "usd",
                    Description = "Order ID: " + ShoppingCartVM.OrderHeader.Id,
                    Source = stripeToken
                };
                var service = new ChargeService();
                Charge charge = service.Create(options);

                if(charge.Id==null)
                {
                    ShoppingCartVM.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusRejected;
                    ShoppingCartVM.OrderHeader.OrderStatus = StaticDetails.StatusPending;
                }
                else
                {
                    ShoppingCartVM.OrderHeader.TransactionId = charge.Id;
                }
                if(charge.Status.ToLower()=="succeeded")
                {
                    ShoppingCartVM.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusApproved;
                    ShoppingCartVM.OrderHeader.OrderStatus = StaticDetails.StatusApproved;
                    ShoppingCartVM.OrderHeader.PaymentDate = DateTime.Now;
                }
            }
            _unitOfWork.Save();
           


            return RedirectToAction("OrderConfirmation","Cart",new { id=ShoppingCartVM.OrderHeader.Id});
        }

        public IActionResult OrderConfirmation(int id)
        {
            return View(id);
        }



    }
}
