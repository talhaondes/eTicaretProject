using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTicaretData.ViewModels
{
    public class AddAdressModel
    {

        [Required(ErrorMessage = "Adres Satırı zorunludur.")]
        public string AddressLine1 { get; set; }

        [Required(ErrorMessage = "Semt/İlçe zorunludur.")]
        public string District { get; set; }

        [Required(ErrorMessage = "Şehir zorunludur.")]
        public string City { get; set; }

        public string AddressType { get; set; }
    }
}
