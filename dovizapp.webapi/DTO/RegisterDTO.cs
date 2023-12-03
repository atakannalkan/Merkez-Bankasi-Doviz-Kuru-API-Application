using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace dovizapp.webapi.DTO
{
    public class RegisterDTO
    {
        public int Id { get; set; }

        [DisplayName("İsim")]
        [Required(ErrorMessage = "'{0}' alanı boş bırakılamaz !")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "'{0}' alanı 3 ile 20 karakter arasında olmalıdır !")]
        public string FirstName { get; set; }

        [DisplayName("Soyisim")]
        [Required(ErrorMessage = "'{0}' alanı boş bırakılamaz !")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "'{0}' alanı 3 ile 20 karakter arasında olmalıdır !")]
        public string LastName { get; set; }

        [DisplayName("Kullanıcı Adı")]
        [Required(ErrorMessage = "'{0}' alanı boş bırakılamaz !")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "'{0}' alanı 3 ile 15 karakter arasında olmalıdır !")]
        public string UserName { get; set; }

        [DisplayName("Şifre")]
        [Required(ErrorMessage = "'{0}' alanı boş bırakılamaz !")]
        [StringLength(20,MinimumLength = 5, ErrorMessage = "{0} 5 ile 20 karakter arasında olmalıdır !")]
        public string Password { get; set; }

        [DisplayName("Şifre Tekrar")]
        [Required(ErrorMessage = "'{0}' alanı boş bırakılamaz !")]
        [Compare(nameof(Password), ErrorMessage = "Şifreler uyuşmuyor !")]
        public string RePassword { get; set; }
    }
}