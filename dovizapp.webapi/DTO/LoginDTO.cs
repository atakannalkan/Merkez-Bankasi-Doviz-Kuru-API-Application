using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace dovizapp.webapi.DTO
{
    public class LoginDTO
    {
        [DisplayName("Kullanıcı Adı")]
        [Required(ErrorMessage = "'{0}' alanı boş bırakılamaz !")]
        public string UserName { get; set; }

        [DisplayName("Şifre")]
        [Required(ErrorMessage = "'{0}' alanı boş bırakılamaz !")]
        public string Password { get; set; }

        public bool RememberMe { get; set; } = false;
    }
}