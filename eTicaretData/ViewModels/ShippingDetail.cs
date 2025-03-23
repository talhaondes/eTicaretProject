using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTicaretData.ViewModels
{
    public class ShippingDetail
    {
        public string UserName { get; set; }

        [Required(ErrorMessage = "Adres Başlığı alanı zorunludur.")]
        public string AdressTitle { get; set; }

        [Required(ErrorMessage = "Adres Satırı zorunludur.")]
        public string AddressLine1 { get; set; }

        [Required(ErrorMessage = "Semt/İlçe alanı zorunludur.")]
        public string District { get; set; }

        [Required(ErrorMessage = "Şehir alanı zorunludur.")]
        public string City { get; set; }
        public string Email { get; set; }



    }


}
