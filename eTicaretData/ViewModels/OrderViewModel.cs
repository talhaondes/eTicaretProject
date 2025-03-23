using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTicaretData.ViewModels
{
    public class OrderViewModel
    {
        public string OrderNumber { get; set; }
        public decimal TotalPrice { get; set; }
        public string AdressTitle { get; set; }
        public string City { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderState { get; set; }
        public string UserName { get; set; }
        public int OrderId { get; set; }  // Bu alan fatura görüntüleme linkinde kullanılacak


    }
}
