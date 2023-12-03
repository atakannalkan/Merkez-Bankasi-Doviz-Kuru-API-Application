using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dovizapp.webui.Models
{
    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public bool RememberMe { get; set; } = false;
    }
}