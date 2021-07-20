using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SarVol.Models
{
    public class ShoppingCart
    {
        public ShoppingCart()
        {
            Count = 1;
        }


        [Key]
        public int Id { get; set; }

        public string AppUserId { get; set; }
        [ForeignKey("AppUserId")]
        public AppUser AppUser { get; set; }



        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        
        [Range(1,1000,ErrorMessage ="Enter a value")]
        public int Count { get; set; }

        [NotMapped]
        public double Price { get; set; }

        [NotMapped]
        public double FullPrice { get; set; }




    }
}
