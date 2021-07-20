using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SarVol.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        
        [Required]
        public string ProductName { get; set; }

        [Required]
        public int Portions { get; set; }

        [Required]
        [Display(Name ="Weight(g)")]
        public float Weight  { get; set; }

        [Required]
        public string Description { get; set; }

        

        [Required]
        [Range(1,1000)]
        public double?  RegularPrice { get; set; }



        
        [Range(1, 1000)]
        public double? PromotionalPrice { get; set; }

        [NotMapped]
        public double PriceToDisplay { get; set; }






        public string ImageUrl { get; set; }



        [Required]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }




        [Required]
        public int ManufacturerId { get; set; }
        [ForeignKey("ManufacturerId")]
        public Manufacturer Manufacturer { get; set; }



        [Required]
        public int TasteId { get; set; }
        [ForeignKey("TasteId")]
        public Taste Taste { get; set; }






    }
}
