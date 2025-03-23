using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTicaretData.Entities
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string? SecondImage { get; set; }     // İkinci fotoğraf
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public bool IsApproved { get; set; }
        public bool IsHome { get; set; }
        public int CategoryId { get; set; }
        public Category category { get; set; }

    }
}
