using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SarVol.Models.ViewModels
{
    public class ProductViewModel
    {
        public Product Product { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
        public IEnumerable<SelectListItem> Manufacturers { get; set; }

        public IEnumerable<SelectListItem> Tastes { get; set; }
    }
    
}
