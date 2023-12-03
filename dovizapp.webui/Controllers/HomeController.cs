using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using dovizapp.business.Abstract;
using dovizapp.data.Abstract;
using dovizapp.data.Concrete.AdoNet.Repositories;
using dovizapp.data.Concrete.EfCore.Repositories;
using dovizapp.entity;
using dovizapp.webui.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace dovizapp.webui.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        public HomeController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        // ** METHODLARDA YAPILAN ORTAK IŞLEMLER !
        // 1- API'dan gelen cevap başarılı ise "Response Content" değeri alındı.
        // 2- "Response Content" değeri JSON'dan Object'e Convert edildi.
        // 3- Bu bilgiler Model'a, oradan da View'a gönderildi.

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CurrencyList()
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {HttpContext.Session.GetString("token")}"); // Request Header
            var response = await _httpClient.GetAsync("http://localhost:4000/api/currency");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {                
                var model = new CurrencyLisyModel() {
                    Currencies = JsonConvert.DeserializeObject<List<Currency>>(responseContent)
                };

                ViewBag.SuccessMessage = "Bütün döviz bilgileri başarıyla listelendi !";
                return View("Index", model);
            }

            ViewBag.Error = $"{response.StatusCode} - {responseContent}";
            return View("Index");
        }

        public async Task<IActionResult> GetCurrencyById(int id)
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {HttpContext.Session.GetString("token")}"); // Request Header
            var response = await _httpClient.GetAsync($"http://localhost:4000/api/currency/{id}");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var model = new CurrencyLisyModel() {
                    Currency = JsonConvert.DeserializeObject<Currency>(responseContent)
                };

                ViewBag.SuccessMessage = $"{id} Id'ye sahip döviz bilgisi başarıyla listelendi !";
                return View("Index", model);
            }

            ViewBag.Error = $"{response.StatusCode} - {responseContent}";
            return View("Index");
        }

        [HttpGet]
        public IActionResult CurrencyCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CurrencyCreate(CurrencyModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {HttpContext.Session.GetString("token")}"); // Request Header
            var postContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"); // Request Body

            var response = await _httpClient.PostAsync("http://localhost:4000/api/currency", postContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var currencyLisyModel = new CurrencyLisyModel() {
                    Currency = JsonConvert.DeserializeObject<Currency>(responseContent)
                };

                ViewBag.SuccessMessage = "Döviz bilgisi başarıyla eklendi !";
                return View("Index", currencyLisyModel);
            }

            ViewBag.Error = $"{response.StatusCode} - {responseContent}";
            return View("Index");
        }

        [HttpGet]
        public async Task<IActionResult> CurrencyEdit(int id)
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {HttpContext.Session.GetString("token")}"); // Request Header

            var response = await _httpClient.GetAsync($"http://localhost:4000/api/currency/{id}");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var currency = JsonConvert.DeserializeObject<Currency>(responseContent);
                var model = new CurrencyModel() {
                    CurrencyId = currency.CurrencyId,
                    CurrencyCode = currency.CurrencyCode,
                    Name = currency.Name,
                    Unit = currency.Unit,
                    ForexBuying = currency.ForexBuying,
                    ForexSelling = currency.ForexSelling,
                    BanknoteBuying = currency.BanknoteBuying,
                    BanknoteSelling = currency.BanknoteSelling,
                };
                
                return View(model);
            }

            ViewBag.Error = $"{response.StatusCode} - {responseContent}";
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> CurrencyEdit(CurrencyModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {HttpContext.Session.GetString("token")}"); // Request Header
            var putContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"); // Request Body

            var response = await _httpClient.PutAsync($"http://localhost:4000/api/currency/{model.CurrencyId}", putContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var currencyModel = new CurrencyLisyModel() {
                    Currency = JsonConvert.DeserializeObject<Currency>(responseContent)
                };

                ViewBag.SuccessMessage = $"{model.CurrencyId} Id'ye sahip döviz bilgisi başarıyla güncellendi !";
                return View("Index", currencyModel);
            }

            ViewBag.Error = $"{response.StatusCode} - {responseContent}";
            return View("Index");            
        }


        public async Task<IActionResult> CurrencyDelete(int id)
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {HttpContext.Session.GetString("token")}"); // Request Header
            var response = await _httpClient.DeleteAsync($"http://localhost:4000/api/currency/{id}");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                ViewBag.SuccessMessage = $"{id} Id'ye sahip döviz bilgisi başarıyla silindi !";
                return View("Index");
            }

            ViewBag.Error = $"{response.StatusCode} - {responseContent}";
            return View("Index");
        }
    }
}