using Microsoft.AspNetCore.Mvc;
using SarVol.DataAccess.Repository.IRepository;
using SarVol.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SarVol.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AboutController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;


        public AboutController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        

        public IActionResult Index()
        {
            IEnumerable<Facility> facilit = _unitOfWork.Facility.GetAll();

           

            return View(facilit);
        }
    }
}
