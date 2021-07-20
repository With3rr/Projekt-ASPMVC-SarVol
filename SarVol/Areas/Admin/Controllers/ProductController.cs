using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SarVol.DataAccess.Repository.IRepository;
using SarVol.Models;
using SarVol.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SarVol.Utility;

namespace SarVol.Areas.Admin.Controllers
{
   

    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnviroment;

        public ProductController(IUnitOfWork unitOfWork,IWebHostEnvironment hostEnviroment)
        {
            _unitOfWork = unitOfWork;
            _hostEnviroment = hostEnviroment;
        }
        public IActionResult Index()
        {

            return View();
        }


        #region Api CAlls
        [HttpGet]
        public IActionResult GetAll()
        {

            var allObj = _unitOfWork.Product.GetAll(includeProperties:"Category,Manufacturer,Taste");
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Product.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error  while deleting" });
            }
            string rootPath = _hostEnviroment.WebRootPath;
            var imagePath = Path.Combine(rootPath,objFromDb.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            _unitOfWork.Product.Remove(objFromDb);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion

        public IActionResult Upsert(int? id)
        {
            ProductViewModel productVM = new ProductViewModel()
            {
                Product = new Product(),
                Categories = _unitOfWork.Category.GetAll().Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() }),
                Manufacturers = _unitOfWork.Manufacturer.GetAll().Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() }),
                Tastes=_unitOfWork.Tastes.GetAll().Select(i=>new SelectListItem { Text=i.Name,Value=i.Id.ToString() })

            };
            if (id == null)
            {
                //create
                return View(productVM);
            }
            //edit
            productVM.Product = _unitOfWork.Product.Get(id.GetValueOrDefault());

            if (productVM.Product == null)
            {
                return NotFound();
            }
            return View(productVM);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductViewModel productVM)
        {
            if (ModelState.IsValid)
            {
                string rootPath = _hostEnviroment.WebRootPath;
                

                var files = HttpContext.Request.Form.Files;

                if(files.Count>0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(rootPath, @"images\products");
                    var extension = Path.GetExtension(files[0].FileName);

                    if(productVM.Product.ImageUrl!=null)
                    {
                        ///this is an edit and we need to remove old image
                        ///

                        var imagePath = Path.Combine(rootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if(System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }

                    }
                    using (var fileStreams = new FileStream(Path.Combine(uploads,fileName+extension),FileMode.Create))
                    {
                        files[0].CopyTo(fileStreams);
                    }
                    productVM.Product.ImageUrl = @"\images\products\" + fileName + extension;

                }
                else
                {
                    //Update when they do not change the image
                    if(productVM.Product.Id!=0)
                    {
                        Product obj = _unitOfWork.Product.Get(productVM.Product.Id);
                        productVM.Product.ImageUrl = obj.ImageUrl;
                    }
                }
                
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);

                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);

                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {

                productVM.Categories = _unitOfWork.Category.GetAll().Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() });
                productVM.Manufacturers = _unitOfWork.Manufacturer.GetAll().Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() });
                if (productVM.Product.Id != 0)
                    productVM.Product = _unitOfWork.Product.Get(productVM.Product.Id);
            }
            return View(productVM);
        }



    }
}
