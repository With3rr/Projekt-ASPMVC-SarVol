using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SarVol.DataAccess.Repository.IRepository;
using SarVol.Models;
using SarVol.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SarVol.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
    public class CategoriesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
                
        }
        public IActionResult Index()
        {

            return View();
        }


        #region Api CAlls
        [HttpGet]
        public IActionResult GetAll()
        {
         
            var allObj = _unitOfWork.Category.GetAll();
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Category.Get(id);
            if(objFromDb==null)
            {
                return Json(new { success = false, message = "Error  while deleting" });
            }
            _unitOfWork.Category.Remove(objFromDb);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion

        public IActionResult Upsert(int? id)
        {
            Category category = new Category();
            if(id==null)
            {
                //create
                return View(category);
            }
            //edit
            category = _unitOfWork.Category.Get(id.GetValueOrDefault());

            if(category==null)
            {
                return NotFound();
            }
            return View(category);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if(ModelState.IsValid)
            {
                if(category.Id==0)
                {
                    _unitOfWork.Category.Add(category);
                    
                }
                else
                {
                    _unitOfWork.Category.Update(category);
                   
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }



    }
}
