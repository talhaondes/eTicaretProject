using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTicaretData.Identity;
using eTicaretData.ViewModels;


namespace eTicaretData.Entities
{
    public class Order
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public decimal TotalPrice { get; set; }
        public string Username { get; set; }  // Kayıtlı kullanıcılar için
        public string AdressTitle { get; set; }
        public string AddressLine1 { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public DateTime OrderDate { get; set; }
        public EnumOrderState OrderState { get; set; }
        public virtual List<OrderLine> OrderLines { get; set; }
    }
}
