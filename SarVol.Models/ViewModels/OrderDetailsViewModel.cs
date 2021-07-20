using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SarVol.Models.ViewModels
{
    public class OrderDetailsViewModel
    {
        public OrderHeader Header { get; set; }

        public IEnumerable<OrderDetails> Details { get; set; }
    }
}
