using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTicaretData.Entities;

namespace eTicaretData.ViewModels
{
    public class ProfileViewModel
    {
        public string Name { get; set; }
        public string SurName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public List<OrderViewModel> Orders { get; set; }
        public List<OrderViewModel> AllOrders { get; set; }
        public List<Address> Addresses { get; set; }



    }
}
