using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagement.Models
{
    public class OrderItem
    {

        public int Id { get; set; }

        [Required]
        public int Quantity { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int ItemId { get; set; }
        public Item Item { get; set; }

        [DefaultValue("getdate()")]
        public DateTime Date_created { get; set; }
        
        //[ConcurrencyCheck]
        [DefaultValue("getdate()")]
        public DateTime Date_updated { get; set; }


        public OrderItem()
        {

        }

    }
}
