using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dovizapp.business.Abstract;
using dovizapp.business.Concrete;
using dovizapp.data.Abstract;
using dovizapp.data.Concrete;
using dovizapp.data.Concrete.AdoNet.Repositories;
using dovizapp.data.Concrete.EfCore.Contexts;
using dovizapp.data.Concrete.EfCore.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace dovizapp.webui
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient(); // HttpClient Inject
            services.AddSession(options => // Session servisi ve ayarları eklendi.
            {
                options.IdleTimeout = TimeSpan.FromDays(1); // Oturumun otomatik olarak sona ereceği zaman aralığını belirler.
                options.Cookie.HttpOnly = true; // Oturum çerezini HTTP üzerinden erişilemez kılar. Bu, güvenlik açısından önemlidir çünkü oturum verilerinin JavaScript gibi istemci tarafından erişilememesini sağlar.
                options.Cookie.IsEssential = true; // Oturum çerezinin uygulamanın temel işleyişini etkileyen verileri içerdiği anlamına gelir. Özellikle, oturum çerezini devre dışı bırakmanın, uygulamanın beklenmedik şekilde çalışmasına neden olabileceği durumlarda kullanışlıdır.
            });
            
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseSession(); // Session servisi kullanıma hazır hale getirildi.
            app.UseStaticFiles(); // wwwroot klasörü

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "currencyedit", 
                    pattern: "home/currency/edit/{id}",
                    defaults: new {controller="Home",action="CurrencyEdit"}
                );
                endpoints.MapControllerRoute(
                    name: "currencycreate", 
                    pattern: "home/currency/create",
                    defaults: new {controller="Home",action="CurrencyCreate"}
                );
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );

                // endpoints.MapGet("/", async context =>
                // {
                //     await context.Response.WriteAsync("Hello World!");
                // });
            });
        }
    }
}
