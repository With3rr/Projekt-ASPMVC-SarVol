using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SarVol.Models
{
    public class Company
    {
        public int Id { get; set; }

        [Required]
     
        public string Name { get; set; }

   
        public string StreetAdress { get; set; }


     
        public string PostalCode { get; set; }

    
        public string City { get; set; }

   
        public string Country { get; set; }


      
        [Required]
        public string PhoneNumber { get; set; }


       
        [Required]
        public int CompanyDiscount { get; set; }
       
    }
}
