using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dovizapp.business.Abstract;
using dovizapp.business.Concrete;
using dovizapp.data.Abstract;
using dovizapp.data.Concrete;
using dovizapp.data.Concrete.EfCore.Contexts;
using dovizapp.shared.Entities;
using dovizapp.shared.Utilities.Security.Abstract;
using dovizapp.shared.Utilities.Security.Concrete;
using dovizapp.shared.Utilities.Seed;
using dovizapp.webapi.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

namespace dovizapp.webapi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private readonly string MyAllowOrigins = "_myAllowOrigins"; // ** CORS Variable
        
        public void ConfigureServices(IServiceCollection services)
        {
            // ** VERITABANI BAĞLANTI NOTLARI !
            // ** İlk önce IDENTITY yapılanması için API katmanı içerisinde mutlaka bir MIGRATION oluşturulup Veritabanına aktarılması gerekiyor (ADO.NET kullanılsa bile) !
            // ** EF CORE veya ADO.NET teknolojisinden hangisi kullanılacaksa "UNIT OF WORK" bölümünde belirtilmesi gerekiyor !
            // ** Uygulamada ADO.NET kullanılsa dahi CONTEXT'lere, kullanacağımız veritabanına göre mutlaka bir CONNECTION STRING verilmesi gerekiyor (SEED işlemleri için) !
            
            // ** CONNECTION DO DATABASE - BU ALANDA VERITABANI SEÇIMI YAPILMASI ZORUNLUDUR !
            services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(Configuration.GetConnectionString("MsSqlConnection"))); // IDENTITY WITH MSSQL !
            services.AddDbContext<DovizContext>(options => options.UseSqlServer(Configuration.GetConnectionString("MsSqlConnection"))); // APPLICATION WITH MSSQL !
            // services.AddDbContext<ApplicationContext>(options => options.UseMySql(Configuration.GetConnectionString("MySqlConnection"), new MySqlServerVersion(new Version(8,0,33)))); // IDENTITY WITH MYSQL !
            // services.AddDbContext<DovizContext>(options => options.UseMySql(Configuration.GetConnectionString("MySqlConnection"), new MySqlServerVersion(new Version(8,0,33)))); // APPLICATION WITH MYSQL !
      

            services.AddIdentity<shared.Entities.User, IdentityRole>().AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();
            // "AddEntityFrameworkStores": Identity verilerini veritabanına kaydetmeyi ve çekmeyi mümkün kılar. Yani, kullanıcı hesapları, roller, oturumlar ve diğer kimlikle ilgili veriler burada saklanır.
            // "AddDefaultTokenProviders": Kimlik doğrulama sırasında, kullanıcı şifre sıfırlama ve hesap onayları gibi taleplerde benzersiz bir token oluşturur. (JWT Token değil !)

            // Identity Settings
            services.Configure<IdentityOptions>(options=> {
                // password
                options.Password.RequireDigit = true; // Parola içerisinde mutlaka sayısal bir değer olmak zorunda ( false ise gerekmiyor ).
                options.Password.RequireLowercase = false; // Parola içerisinde mutlaka küçük harf olmak zorunda ( false ise gerekmiyor ).
                options.Password.RequireUppercase = false; // Parola içerisinde mutlaka büyük harf olmak zorunda ( false ise gerekmiyor ).
                options.Password.RequiredLength = 5; // Minimum kaç karakterli olsun ?
                options.Password.RequireNonAlphanumeric = false; // Parola içerisinde mutlaka "@, _" gibi karakterler olmak zorunda ( false ise gerekmiyor ).

                // Lockout
                options.Lockout.MaxFailedAccessAttempts = 5; // Kullanıcı yanlış parola hakkıdır, belirtilen değer kadar yanlış girilirse hesap kilitlenir ( false ise gerekmiyor ).
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1); // belirtilen dakika sonra ise açılır.
                options.Lockout.AllowedForNewUsers = true; // Lockout'un aktif olması için buna true vermemiz gerekiyor.

                // options.User.AllowedUserNameCharacters = ""; // Kullanıcı UserName'si içerisinde olması gerekeni buraya yazabilirsin.
                options.User.RequireUniqueEmail = false; // Her kullanıcının birbirinden farklı E-Mail hesabı olması gerekiyor aynı Mail ile birden fazla kullanıcı oluşturulamaz.
                options.SignIn.RequireConfirmedEmail = false; // Bir kullanıcı üye olur ve mutlaka Mail'ini onaylanması gerekiyor.
                options.SignIn.RequireConfirmedPhoneNumber = false; // Bir kullanıcı üye olur ve mutlaka Telefonu onaylanması gerekiyor.
            });
            

            // ** DEPENDENCY INJECTION
            services.AddScoped<ICurrencyService, CurrencyManager>();
            services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
            
            // DI(Dependency Injection) ile, JSON dosyasından okuduğumuz verileri bir model veya sınıfa "MAP" ediyoruz !
            services.Configure<JwtSettingsModel>(Configuration.GetSection("JwtSettings"));          

            // ** UNIT OF WORK
            services.AddScoped<IUnitOfWork, EfCoreUnitOfWork>();
            // services.AddScoped<IUnitOfWork, AdoNetUnitOfWork>(options => new AdoNetUnitOfWork(Configuration.GetConnectionString("MsSqlConnection")));


            var jwtConfiguration = Configuration.GetSection("JwtSettings");            
            // ** JWT MIDDLEWARE CONFIGURATIONS !
            // ** Uygulamaya her Token gönderdiğimizde, Token içerisindeki bilgiler ile, aşağıda yapmış olduğumuz Configuration bilgileri ile eşleşiyor mu diye kontrolü sağlanır. Ardından "Authorization" işlemi gerçekleşir !
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {

                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwtConfiguration["Issuer"],
                    ValidAudience = jwtConfiguration["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration["SecretKey"])),
                    ClockSkew = TimeSpan.Zero
                };
            });
            

            services.AddHttpClient(); // HttpClient Inject

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "dovizapp.webapi", Version = "v1" });
            });

            // ** CORS (Cross-Origin Resource Sharing)            
            services.AddCors(options => {
                options.AddPolicy(
                    name: MyAllowOrigins,
                    policy => {
                        policy
                        // .WithOrigins("http://127.0.0.1:5500/") // Sadece belirtilen adresteki istekler kabul edilir.
                        .WithHeaders(HeaderNames.ContentType, "x-custom-header") // Header bilgisinde, ilgili parametre varsa çalışır.
                        .AllowAnyOrigin() // Bütün talepler karşılanır.
                        .AllowAnyHeader() // Header bilgisi, yani belirli parametreler olması gerekir.
                        .AllowAnyMethod(); // Sadece belirli Request'leri karşılayabiliriz(Create, Update).
                    }
                );
            });
        }

        // DovizContext'i ConfigureServices metodunda DI(Dependency Injection)'ye eklemiş olduğumuz için, Configure metodu içinde parametre olarak kullanabiliriz.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DovizContext dovizContext, UserManager<shared.Entities.User> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "dovizapp.webapi v1"));
            }

            // "Configure" methodunu "await" ile bekletmek tavsiye edilmez çünkü; Uygulamanın başlangıcını hızlandırmak için, hızlıca çalışması beklenir. 
            SeedDatabase.Seed(dovizContext, Configuration).Wait(); // Seed işlemini başlatıyoruz ancak Configure'u bekletmiyoruz.
            SeedIdentity.Seed(userManager, roleManager, Configuration).Wait();

            // app.UseHttpsRedirection(); // ** I CLOSED THE SSL CERTIFICATE

            app.UseRouting();

            app.UseCors(MyAllowOrigins);

            app.UseAuthentication(); // Authentication for JWT !
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
