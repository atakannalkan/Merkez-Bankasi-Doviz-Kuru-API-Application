# ASP.NET Core ile Merkez Bankası Döviz Kuru API Uygulaması

## Uygulamaya Genel Bakış

Bu uygulama, ASP.NET  Core kullanılarak geliştirilmiş, TC Merkez Bankası'ndan alınan döviz kuru bilgilerinin ön yüze aktarıldığı bir **API** uygulamasıdır. Uygulama ile, kullanıcılar siteye girerek kayıt olabilir, giriş yapabilir, anasayfadan döviz kurlarını listeleyebilir. Ek olarak Döviz kurlarına ekleme, silme ve güncelleme olarak adlandırdığımız **CRUD** (Create, Read, Update, Delete) işlemlerini de kolaylıkla yapabilir.

Uygulama genel hatlarıyla bir Back-End uygulaması olduğu için, Front-End tarafında çok fazla kodlama yapılmamıştır. Bunun dışında, uygulamanın nasıl bir yazılım mimarisi ile geliştirildiği ve nasıl kullanılacağı ile ilgili detaylı bilgiler aşağıda verilmiştir.

## Uygulama Konu Başlıkları

* Generic Repository Pattern
* Unit Of Work Design Pattern
* Entity Framework Core
* ADO.NET
* Fluent API
* ASP.NET Core Identity
* JWT (JSON Web Token)
* Dependency Injection
* Stored Procedure
* Web API (Get-Post-Put-Delete)
* Web API DTO (Data Transfer Object)
* Web API Cors (Cross-Origin Resource Sharing)

---

## Back-End ve Veritabanı

Uygulamanın Back-End kısmında, Veritabanı olarak hem MsSQL hem de MySQL Veritabanları kullanılmıştır. Veritabanı sorgu yönetiminde ise .NET Core ile birlikte gelen Entity Framework Core aracındaki LINQ (Language Integrated Query) sorguları ile işlemler gerçekleştirilmiştir.

Ayrıca Veritabanı işlemleri için EF Core ile gerçekleştirilen işlemler, ADO.NET mimarisi altında da geliştirilmiştir. Bu sayede veritabanı işlemleri için hem EF Core hem de ADO.NET teknolojisinin kullanımına olanak sağlanmıştır.

Uygulamanın API katmanına erişmek için Authentication (Kimlik Doğrulama) bölümünde ASP.NET Core Identity, Authorization (Yetkilendirme) bölümünde ise JWT (JSON Web Token) kullanılmıştır. Bu güvenlik yapıları sayesinde, kimliğini doğrulamayan kullanıcılar API katmanına erişim sağlayamayacaktır.

## Uygulamayı Çalıştırmak

Projeyi sıfırdan çalıştırmak için ilgili dosyaları indirip açtıktan sonra yapılması gereken ilk işlem Veritabanı bağlantısıdır. Veritabanı bağlantısı "**dovizapp.webapi**" katmanı içerisindeki "**appsettings.json > ConnectionStrings**" adresi üzerinde yer almaktadır. Bu Veritabanı adresinin, Database Context'e iletildiği kod satırı ise "**Startup > ConfigureServices**" adresi içerisinde yer almaktadır. Bu adres üzerinde, kullanılacak olan Veritabanına göre (Örn: MsSQL, MySQL vb...) bağlantı ayarları düzenlenip yapılanıdılabilir.

Bu bağlantı, uygulama içerisinde varsayılan olarak **MsSQL** Veritabanı ile yapılmıştır, eğer mevcut bilgisayarda **MsSQL**  Veritabanı yoksa uygulama Veritabanına bağlanamayacak ve hata verecektir. Eğer uygulama başka bir Veritabanında çalıştırılmak isteniyorsa (Örn: MySQL) "**dovizapp.webapi**" ve "**dovizapp.data**" katmanları içerisindeki **Migrations** klasörlerinin silinip, yeni bir Migration oluşturulması gerekmektedir.