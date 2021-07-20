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
    public class FacilityController : Controller
    {
       

        private readonly IUnitOfWork _unitOfWork;

        public FacilityController(IUnitOfWork unitOfWork)
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

            var allObj = _unitOfWork.Facility.GetAll();
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Facility.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error  while deleting" });
            }
            _unitOfWork.Facility.Remove(objFromDb);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion

        public IActionResult Upsert(int? id)
        {
            Facility facility = new Facility();
            if (id == null)
            {
                //create
                return View(facility);
            }
            //edit
            facility = _unitOfWork.Facility.Get(id.GetValueOrDefault());

            if (facility == null)
            {
                return NotFound();
            }
            return View(facility);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Facility facility)
        {
            if (ModelState.IsValid)
            {
                if (facility.Id == 0)
                {
                    _unitOfWork.Facility.Add(facility);

                }
                else
                {
                    _unitOfWork.Facility.Update(facility);

                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(facility);
        }
    }
}
