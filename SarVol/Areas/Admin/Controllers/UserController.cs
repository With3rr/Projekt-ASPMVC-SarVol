using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SarVol.DataAccess.Data;
using SarVol.DataAccess.Repository.IRepository;
using SarVol.Models.ViewModels;
using SarVol.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SarVol.Areas.Admin.Controllers
{
    
   
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
    public class UserController : Controller
    {
        
       
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;

        public UserController(IUnitOfWork unitOfWork,ApplicationDbContext db)
        {
            _db = db;
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
           

            var allObj = _unitOfWork.AppUser.GetAll(includeProperties:"Company");
            _db.UserRoles.ToList();
            _db.Roles.ToList();
            var userRoles = _unitOfWork.UserRoles.GetAll();
            var roles = _unitOfWork.Roles.GetAll();

            foreach (var item in allObj)
            {
                var roleId = userRoles.FirstOrDefault(i => i.UserId == item.Id).RoleId;
                item.Role = roles.FirstOrDefault(i => i.Id == roleId).Name;
                if (item.Company == null)
                    item.Company = new Models.Company() { Name = "" };

            }
            return Json(new { data = allObj });
        }
        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id )
        {
            var obj = _unitOfWork.AppUser.GetFirstOrDefault(u => u.Id == id);
            if(obj==null)
            {
                return Json(new { success = false, message = "Error" });
            }
            if(obj.LockoutEnd!=null && obj.LockoutEnd>DateTime.Now)
            {
                //User is locked,we will unlock them
                obj.LockoutEnd = DateTime.Now;
            }
            else
            {
                obj.LockoutEnd = DateTime.Now.AddYears(1);
            }
            _unitOfWork.Save();
            return Json(new { success = true, message = "Operation successful" });



        }

        //[HttpDelete]
        //public IActionResult Delete(int id)
        //{
        //    var objFromDb = _unitOfWork.Category.Get(id);
        //    if (objFromDb == null)
        //    {
        //        return Json(new { success = false, message = "Error  while deleting" });
        //    }
        //    _unitOfWork.Category.Remove(objFromDb);
        //    _unitOfWork.Save();

        //    return Json(new { success = true, message = "Delete Successful" });
        //}
        #endregion

        



    }
}
