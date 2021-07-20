using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SarVol.Models.ViewModels
{
    public class ShoppingCartViewModel
    {
        public IEnumerable<ShoppingCart> Shoppings { get; set; }

        public OrderHeader OrderHeader { get; set; }

        public int Count { get; set; }
    }
}
