using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using dovizapp.entity;
using dovizapp.webui.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace dovizapp.webui.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        public AccountController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var postContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"); // Request Body

            var response = await _httpClient.PostAsync("http://localhost:4000/api/auth/register", postContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = responseContent;
                return RedirectToAction("Login");
            }
            
            ModelState.AddModelError("", responseContent);
            return View(model);
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var postContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"); // Request Body

            var response = await _httpClient.PostAsync("http://localhost:4000/api/auth/login", postContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                HttpContext.Session.SetString("token", responseContent);
                return RedirectToAction("Index","Home");
            }
                
            ModelState.AddModelError("", responseContent);
            return View(model);
        }


        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Tüm Session değerlerini siliyoruz.
            return RedirectToAction("Index","Home");
        }
    }
}