using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SarVol.DataAccess.Repository.IRepository;
using SarVol.Models;
using SarVol.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SarVol.Utility;

namespace SarVol.Areas.Customer.Controllers
{

    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger,IUnitOfWork unitOfWork)
        {
            
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category,Manufacturer,Taste");

            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim!=null)
            {
                var count = _unitOfWork.ShoppingCart.GetAll(i => i.AppUserId == claim.Value).ToList().Count();
                HttpContext.Session.SetObject(StaticDetails.SesionShoppingCart, count);
            }

            return View(products);
        }

       
        public IActionResult Details(int id)
        {
            var product = _unitOfWork.Product.GetFirstOrDefault(i => i.Id == id,includeProperties:"Manufacturer,Category,Taste");

            ShoppingCart cart = new ShoppingCart()
            {
                Product = product,
                ProductId = product.Id
            };
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart cartProd)
        {
            cartProd.Id = 0;

            if(ModelState.IsValid)
            {
                var identity = (ClaimsIdentity)User.Identity;
                var claim = identity.FindFirst(ClaimTypes.NameIdentifier);
                cartProd.AppUserId = claim.Value;


                ShoppingCart db = _unitOfWork.ShoppingCart.GetFirstOrDefault(i => i.ProductId == cartProd.ProductId && i.AppUserId == cartProd.AppUserId, includeProperties: "Product");
                if(db==null)
                {
                    //no records for that product for this user

                    _unitOfWork.ShoppingCart.Add(cartProd);
                }
                else
                {
                    db.Count += cartProd.Count;
                    _unitOfWork.ShoppingCart.Update(db);
                }
                _unitOfWork.Save();

                var count = _unitOfWork.ShoppingCart.GetAll(i => i.AppUserId == cartProd.AppUserId).ToList().Count();

                HttpContext.Session.SetObject(StaticDetails.SesionShoppingCart, count);

                return RedirectToAction(nameof(Index));

                //then we will add to cart
            }
            else
            {
                var product = _unitOfWork.Product.GetFirstOrDefault(i => i.Id == cartProd.Product.Id, includeProperties: "Manufacturer,Category");

                ShoppingCart cart = new ShoppingCart()
                {
                    Product = product,
                    ProductId = product.Id
                };
                return View(cart);
            }


           
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
