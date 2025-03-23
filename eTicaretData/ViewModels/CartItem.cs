using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTicaretData.Entities;

namespace eTicaretData.ViewModels
{
    public class CartItem
    {
        public Product product {  get; set; }   
        public int Quantity { get; set; }
        public int ProductId { get; set; }
    }
}
