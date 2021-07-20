using Dapper;
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
    public class ManufacturesController : Controller
    {
        private IUnitOfWork _unitOfWork;
        public ManufacturesController(IUnitOfWork unit)
        {
            _unitOfWork = unit;

        }
        public IActionResult Index()
        {
            return View();
        }



        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var objects = _unitOfWork.Manufacturer.GetAll();
            return Json(new { data = objects });
        }


        [HttpDelete]
        public IActionResult Delete(int id)
        {

            var objFromDb = _unitOfWork.Manufacturer.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error  while deleting" });
            }
            _unitOfWork.Manufacturer.Remove(objFromDb);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });





            
        }


        #endregion

        public IActionResult Upsert(int? id)
        {


    


            Manufacturer manufacturer = new Manufacturer();

            if(id==null)
            {
                return View(manufacturer);
            }
            manufacturer = _unitOfWork.Manufacturer.Get(id.GetValueOrDefault());
            if (manufacturer==null)
            {
                return NotFound();
            }
            
            return View(manufacturer);


        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Upsert(Manufacturer Manufacturer)
        {

            if (ModelState.IsValid)
            {
                if (Manufacturer.Id == 0)
                {
                    _unitOfWork.Manufacturer.Add(Manufacturer);

                }
                else
                {
                    _unitOfWork.Manufacturer.Update(Manufacturer);

                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(Manufacturer);





           

        }


    }
}
