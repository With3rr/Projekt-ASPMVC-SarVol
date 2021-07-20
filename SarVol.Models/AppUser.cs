using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SarVol.Models
{
    public class AppUser:IdentityUser
    {
        
        [Required]
        public string Name { get; set; }

        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }

        [NotMapped]
        public string Role { get; set; }


        public int? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company Company { get; set; }


    }
}
