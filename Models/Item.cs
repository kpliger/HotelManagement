
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
    public class Item
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Required] 
        public string Description { get; set; }

        public int Quantity { get; set; }
        public int QuantityPar { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal Price { get; set; }
        public string creatorId { get; set; }
        [DefaultValue(1)]
        public int active { get; set; }
        [DefaultValue("getdate()")]
        public DateTime date_created { get; set; }
        //[ConcurrencyCheck]
        [DefaultValue("getdate()")]
        public DateTime date_updated { get; set; }

        public IdentityUser creator { get; set; }

        public ICollection<ItemImage> ItemImages { get; set; }

        public Item()
        {

        }
    }

}
