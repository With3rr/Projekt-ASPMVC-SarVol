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
    public class TasteController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public TasteController(IUnitOfWork unitOfWork)
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

            var allObj = _unitOfWork.Tastes.GetAll();
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Tastes.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error  while deleting" });
            }
            _unitOfWork.Tastes.Remove(objFromDb);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion

        public IActionResult Upsert(int? id)
        {
            Taste category = new Taste();
            if (id == null)
            {
                //create
                return View(category);
            }
            //edit
            category = _unitOfWork.Tastes.Get(id.GetValueOrDefault());

            if (category == null)
            {
                return NotFound();
            }
            return View(category);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Taste taste)
        {
            if (ModelState.IsValid)
            {
                if (taste.Id == 0)
                {
                    _unitOfWork.Tastes.Add(taste);

                }
                else
                {
                    _unitOfWork.Tastes.Update(taste);

                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(taste);
        }
    }
}
