using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using dovizapp.business.Abstract;
using dovizapp.entity;
using dovizapp.shared.Utilities.Security.Abstract;
using dovizapp.webapi.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace dovizapp.webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize] // Sade bir şekilde kullanırsak, Authorize işlemini JWT ile değil de ASP.NET CORE Identity ile yapmaya çalışır ve "Access Denied" hatası alırız. API katmanında da View olmadığı için bizlere "404 Not Found" hatası döner !
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Authentication işlemini JWT ile yapmak istediğimizi belirttik.

    // ControllerBase == There is no view !
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public CurrencyController(ICurrencyService currencyService, HttpClient httpClient, IConfiguration configuration)
        {
            _currencyService = currencyService;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrencies()
        {
            // ** UYGULAMAYA EKLENENLER !              
            // TODO Projeye "Unit Of Work" Design Pattern'ı eklenecek. // ** YAPILDI !
            // TODO Uygulama ilk açıldığında Stored Procedure'ler, Veritabanına otomatik olarak eklenecek (SEED yöntemi ile). // ** YAPILDI !
            // TODO MsSQL ve MySQL'deki Stored Procedure'lere ADO.NET ve EF CORE ile istek atılacak (CRUD işlemleri için). // ** YAPILDI !
            // TODO "Entity" katmanı içerisinde bir Log tablosu oluşturulacak. // ** YAPILDI !
            // TODO "Shared > Extensions" içerisindeki "ReaderToCurrency" methodu güncellenecek. // ** YAPILDI !
            // TODO UI ve JS katmanındaki Button'a tıklanınca, API'deki güncel kur bilgileri önyüze aktarılacak. // ** YAPILDI !            

            // TODO Projeye ASP.NET CORE IDENTITY yapısı eklenecek // ** YAPILDI !
            // TODO Identity için "Seed Identity" yapısı eklenecek // ** YAPILDI !
            // TODO Login (Authentication) işlemi gerçekleştikten sonra, JWT (JSON Web Token) ile geriye bir TOKEN döndürülecek. Takip eden isteklerde ilgili TOKEN geçerliyse Authorize olunup Request'e başarılı bir Response dönülürken, değil ise "401 Unauthorized" dönülecek ! // ** YAPILDI !
            // TODO API katmanındaki JWT'ye role alanı eklenecek // ** YAPILDI !
            


            DateTime dateTime = DateTime.Now; // Güncel tarih bilgisi.
            var latestCurrency = await _currencyService.GetLatestCurrencyAsync(); // Son kayıt.
            var updateHour = 12; // Güncellenecek saat bilgisi.


            // ** Görev: saat 12:00'de veriler güncellenecek, bir sonraki 24 saat'e kadar bir daha değişmeyecek.
            // 1- Kayıt bugün eklendiyse saatin 12'den küçük olması lazım.
            // 2- Kayıt bugün eklenmediyse saate bakılmaksızın, verilerin direkt güncellenmesi gerekiyor.
            if ((latestCurrency == null || (dateTime.Date - latestCurrency.UpdatedDate.Date).Days >= 2) || // Eğer son kayıt Null ise veya bu güncellenen veri iki gün önce güncellendiyse hiçbir koşula bakmadan direkt güncelleme işlemi yapıyoruz.
            (
                (dateTime.Hour >= updateHour) && ((latestCurrency.UpdatedDate.Date == dateTime.Date && latestCurrency.UpdatedDate.Hour < updateHour) // Bugün 12:00'den sonra güncelleme yapıldıysa birdaha güncellemiyoruz.
                || (latestCurrency.UpdatedDate.Date < dateTime.Date) )
            ))
            {
                // ** Step 1: Bütün bilgileri Log tablosuna aktar !
                await _currencyService.TransferAllCurrenciesToLogTableAsync();

                // ** Step 2: Güncel kur bilgilerini al ve veritabanına aktar !
                var currencyList = new List<Currency>();

                var response = await _httpClient.GetAsync("https://www.tcmb.gov.tr/kurlar/today.xml");
                var responseContent = await response.Content.ReadAsStringAsync();

                XDocument doc = XDocument.Parse(responseContent);

                foreach (var currency in doc.Descendants("Currency"))
                {
                    currencyList.Add(new Currency() {
                        CurrencyCode = (string)currency.Attribute("CurrencyCode"),
                        Name = (string)currency.Element("CurrencyName"),
                        Unit = (int)currency.Element("Unit"),
                        ForexBuying = checkNullOrEmpty(currency.Element("ForexBuying")),
                        ForexSelling = checkNullOrEmpty(currency.Element("ForexSelling")),
                        BanknoteBuying = checkNullOrEmpty(currency.Element("BanknoteBuying")),
                        BanknoteSelling = checkNullOrEmpty(currency.Element("BanknoteSelling"))
                    });
                }

                await _currencyService.UpdateAllCurrenciesAsync(currencyList);
            }            
            
            // ** Step 3: Veritabanına kaydolan bilgiler alınacak !
            var currenciesDTO = new List<CurrencyDTO>();
            var currencies = await _currencyService.GetAllUsingProcedureAsync();

            foreach (var currency in currencies)
            {
                currenciesDTO.Add(CurrencyToDTO(currency));
            }

            return Ok(currenciesDTO);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCurrency(int id)
        {
            var currency = await _currencyService.GetByIdUsingProcedureAsync(id);
            if (currency == null)
            {
                return NotFound("Döviz Bulunamadı !"); // 404 Error !
            }
            
            return Ok(CurrencyToDTO(currency)); // 200
        }

        [HttpPost]
        public async Task<IActionResult> CreateCurrency(Currency entity)
        {
            var currency = await _currencyService.CreateUsingProcedureAsync(entity);
            return CreatedAtAction(nameof(GetCurrency), new {id = entity.CurrencyId}, CurrencyToDTO(currency)); // => 201 Response Code
            //return CreatedAtAction("GetCurrency", new {id = entity.Id}, entity); // This is the same as above.
        }
       
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCurrency(int id, Currency entity)
        {
            if (id != entity.CurrencyId)
            {
                return BadRequest("İşlem Hatası - Bilgiler Tutarlı Olmalıdır !"); // 400 Error !
            }

            var currency = await _currencyService.GetByIdUsingProcedureAsync(id);
            if (currency == null)
            {
                return NotFound("Döviz Bulunamadı !");
            }

            currency.CurrencyCode = entity.CurrencyCode;
            currency.Name = entity.Name;
            currency.Unit = entity.Unit;
            currency.ForexBuying = entity.ForexBuying;
            currency.ForexSelling = entity.ForexSelling;
            currency.BanknoteBuying = entity.BanknoteBuying;
            currency.BanknoteSelling = entity.BanknoteSelling;
            currency.UpdatedDate = DateTime.Now;

            await _currencyService.UpdateUsingProcedureAsync(currency);
            return Ok(CurrencyToDTO(currency));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCurrency(int id)
        {
            var currency = await _currencyService.GetByIdUsingProcedureAsync(id);
            if (currency == null)
            {
                return NotFound("Döviz Bulunamadı !");
            }

            await _currencyService.DeleteUsingProcedureAsync(currency);
            return NoContent();
        }


        // ** DTO MAPPING
        private static CurrencyDTO CurrencyToDTO(Currency currency)
        {
            return new CurrencyDTO {
                CurrencyId = currency.CurrencyId,
                CurrencyCode = currency.CurrencyCode,
                Name = currency.Name,
                Unit = currency.Unit,
                ForexBuying = currency.ForexBuying,
                ForexSelling = currency.ForexSelling,
                BanknoteBuying = currency.BanknoteBuying,
                BanknoteSelling = currency.BanknoteSelling
            };
        }
    
        private static double checkNullOrEmpty(XElement number)
        {
            return string.IsNullOrEmpty((string)number) ? 0.00 : (double)number;
        }
    }
}