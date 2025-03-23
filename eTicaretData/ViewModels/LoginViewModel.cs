using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTicaretData.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı alanı zorunludur.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        public string PassWord { get; set; }

        public bool RememberMe { get; set; }
    }
}
