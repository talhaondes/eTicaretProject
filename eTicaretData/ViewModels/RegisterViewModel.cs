using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace eTicaretData.ViewModels
{
    public class RegisterViewModel
    {
        public string Name { get; set; }
        public string SurName { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçersiz Email Adresi")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        public string PassWord { get; set; }

        [Required(ErrorMessage = "Şifre onayı zorunludur.")]
        [Compare("PassWord", ErrorMessage = "Şifre ve şifre onayı eşleşmiyor.")]
        public string ConfirmPassWord { get; set; }
        [Required(ErrorMessage = "Telefon zorunludur.")]
        public string PhoneNumber { get; set; }
    }
}

