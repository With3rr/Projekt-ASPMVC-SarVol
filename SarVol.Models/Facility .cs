using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SarVol.Models
{
    public class Facility
    {
       

        public int Id { get; set; }

        [Required]

        public string Name { get; set; }


        [Required]
        
        public string StreetAdress { get; set; }



        [Required]
     
        public string PostalCode { get; set; }


        [Required]
    
        public string City { get; set; }

        

      
       
        [Required]
        public string PhoneNumber { get; set; }


  
        [Required]
        public string OpenedSaturday { get; set; }


        [Required]
        public string OpenedWeek { get; set; }


        [Required]
        public string MapUrl{ get; set; }








    }
}
