using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dovizapp.webui.ViewComponents
{
    public class NavbarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            bool tokenExist = HttpContext.Session.GetString("token") != null ? true : false;

            return View(tokenExist);
        }
    }
}