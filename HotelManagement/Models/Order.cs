
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagement.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Status { get; set; }
        
        public string CreatorId { get; set; }
        
        public IdentityUser Creator { get; set; }
        
        [DefaultValue(1)]
        public int Active { get; set; }
        
        [DefaultValue("getdate()")]
        public DateTime Date_created { get; set; }
        
        //[ConcurrencyCheck]
        [DefaultValue("getdate()")]
        public DateTime Date_updated { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }

        public Order()
        {

        }
    }

}
