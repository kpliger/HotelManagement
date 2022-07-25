using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagement.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required] 
        public string Name { get; set; }

        [DefaultValue("BASIC")] 
        public string Permission { get; set; }

        public string UserId { get; set; }

        public IdentityUser User { get; set; }

        public Employee()
        {

        }

    }
}
