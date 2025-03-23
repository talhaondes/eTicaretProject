using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTicaretData.Entities
{
    public class OrderLine
    {
        public int OrderLineId { get; set; }
        public int OrderLineQuantity { get; set; }
        public decimal OrderLinePrice { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public int ProductId { get; set; }  
        public virtual Product Product { get; set; }    



    }
}
