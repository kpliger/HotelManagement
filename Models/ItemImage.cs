using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagement.Models
{
    public class ItemImage
    {

        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }
        
        public string Filename { get; set; }
        
        public int ItemId { get; set; }

        [DefaultValue("getdate()")]
        public DateTime date_created { get; set; }
        
        //[ConcurrencyCheck]
        [DefaultValue("getdate()")]
        public DateTime date_updated { get; set; }

        public  Item Item { get; set; }

        public ItemImage()
        {

        }

    }
}
