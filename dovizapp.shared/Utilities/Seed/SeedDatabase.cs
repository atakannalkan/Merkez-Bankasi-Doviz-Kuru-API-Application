using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace dovizapp.shared.Utilities.Seed
{
    public static class SeedDatabase
    {
        public static async Task Seed(DbContext dbContext, IConfiguration configuration)
        {
            if (dbContext.Database.IsSqlServer()) // Bu yöntem sayesinde hangi veritabanında çalıştığımı öğrenebiliyorum.
            {
                await SeedDatabaseForMsSQLAsync(configuration); // Eğer Server'da Database ve Table'lar yoksa ekliyoruz.
                
                await SeedStoredProceduresForMsSQLAsync(dbContext); // Procedure'leri Database'e ekliyoruz.
                
            } else if (dbContext.Database.IsMySql()) {

                await SeedDatabaseForMySQLAsync(configuration);
                
                await SeedStoredProceduresForMySQLAsync(dbContext);                
            }
        }


        // ** DATABASE OPERATIONS - CODE FIRST WITH ADO.NET !
        private static async Task SeedDatabaseForMsSQLAsync(IConfiguration configuration)
        {
            // ** Ado.Net ile "appsettings.json" daki Database bilgisi MsSQL'de mevcut değil ise bunu ekliyoruz.
            // Ancak bunu yapabilmek için önce; Database bilgisinin var olmadığını, "MainConnectionString" bilgisi ile almamız gerekiyor. Çünkü; Normal ConnectionString ile 
            // bağlanmaya çalışırsak veritabanı mevcut olmadığı için Database'e bağlanamayacak ve program çökecektir. Bunun için önce "MainConnectionString" ile Server'da
            // Bu database yok ise, oluşturuyoruz, ardından normal "ConnectionString" ile de Database'e tablolarımızı ekliyoruz.

            var mainConnectionString = configuration.GetConnectionString("AdoNetMainMsSqlConnection");
            var connectionString = configuration.GetConnectionString("MsSqlConnection");

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString); // Bu sınıf, bağlantı dizelerini oluşturmak, düzenlemek ve ayrıştırmak için kullanılır. Aynı zamanda bağlantı dizesinin parçalarına erişim sağlar.
            var databaseName = builder.InitialCatalog; // ConnectionString'den aldığımız database ismini değişkene aktardık. Bu sayede kodumuz dinamik hale geldi.

            using (var connection1 = new SqlConnection(mainConnectionString))
            {
                try
                {
                    await connection1.OpenAsync();
                    
                    SqlCommand command1 = new SqlCommand($"SELECT COUNT(*) FROM sys.databases WHERE name='{databaseName}'", connection1);
                    var commandScalar = (int)await command1.ExecuteScalarAsync();

                    if (commandScalar == 0)
                    {
                        // "sys.databases", SQL Server içinde bulunan tüm veritabanlarının listesini içerir.
                        SqlCommand command2 = new SqlCommand($"IF NOT EXISTS (SELECT name FROM sys.databases WHERE name='{databaseName}') CREATE DATABASE {databaseName}", connection1);
                        await command2.ExecuteNonQueryAsync();
                    }

                    using (var connection2 = new SqlConnection(connectionString))
                    {
                        await connection2.OpenAsync();

                        var query = "";

                        // "sys.tables", SQL Server içinde bulunan tüm tabloların listesini içerir.
                        query += @$"USE {databaseName};
                        IF NOT EXISTS(SELECT * FROM sys.tables WHERE name='Currencies') CREATE TABLE Currencies (CurrencyId INT IDENTITY(1,1) PRIMARY KEY, CurrencyCode NVARCHAR(30) UNIQUE,
                        Name NVARCHAR(100), Unit INT, ForexBuying FLOAT, ForexSelling FLOAT, BanknoteBuying FLOAT, BanknoteSelling FLOAT, CreatedDate DATETIME2(7), UpdatedDate DATETIME2(7));";

                        query += @$"USE {databaseName};
                        IF NOT EXISTS(SELECT * FROM sys.tables WHERE name='CurrencyLogs') CREATE TABLE CurrencyLogs(CurrencyLogId INT IDENTITY(1,1) PRIMARY KEY, CurrencyId INT, CurrencyCode NVARCHAR(30) UNIQUE,
                        Name NVARCHAR(100), Unit INT, ForexBuying FLOAT, ForexSelling FLOAT, BanknoteBuying FLOAT, BanknoteSelling FLOAT, CreatedDate DATETIME2(7), UpdatedDate DATETIME2(7), LogAddedDate DATETIME2(7));";
                        
                        SqlCommand command3 = new SqlCommand(query , connection2);
                        await command3.ExecuteNonQueryAsync();

                        await connection2.CloseAsync();
                    }
                    
                }
                catch (Exception error)
                {
                    Console.WriteLine("ERROR: "+error);
                }
                finally
                {
                    await connection1.CloseAsync();
                }
            }
        }
   
        private static async Task SeedDatabaseForMySQLAsync(IConfiguration configuration)
        {
            var mainConnectionString = configuration.GetConnectionString("AdoNetMainMySqlConnection");
            var connectionString = configuration.GetConnectionString("MySqlConnection");

            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder(connectionString);
            var databaseName = builder.Database;

            using (var connection1 = new MySqlConnection(mainConnectionString))
            {
                try
                {
                    await connection1.OpenAsync();
                    
                    MySqlCommand command1 = new MySqlCommand($"SELECT COUNT(*) FROM information_schema.schemata WHERE schema_name='{databaseName}'", connection1);
                    var commandScalar = Convert.ToInt32(await command1.ExecuteScalarAsync());

                    if (commandScalar == 0)
                    {
                        MySqlCommand command2 = new MySqlCommand($"CREATE DATABASE IF NOT EXISTS {databaseName}", connection1);
                        await command2.ExecuteNonQueryAsync();
                    }

                    using (var connection2 = new MySqlConnection(connectionString))
                    {
                        await connection2.OpenAsync();
                        
                        var query = @$"USE {databaseName};
                        CREATE TABLE IF NOT EXISTS Currencies (CurrencyId INT AUTO_INCREMENT PRIMARY KEY, CurrencyCode VARCHAR(30) UNIQUE,
                        Name VARCHAR(100), Unit INT, ForexBuying DOUBLE, ForexSelling DOUBLE, BanknoteBuying DOUBLE, BanknoteSelling DOUBLE, CreatedDate DATETIME(6), UpdatedDate DATETIME(6));";

                        query += @$"USE {databaseName};
                        CREATE TABLE IF NOT EXISTS CurrencyLogs(CurrencyLogId INT AUTO_INCREMENT PRIMARY KEY, CurrencyId INT, CurrencyCode VARCHAR(30),
                        Name VARCHAR(100), Unit INT, ForexBuying DOUBLE, ForexSelling DOUBLE, BanknoteBuying DOUBLE, BanknoteSelling DOUBLE, CreatedDate DATETIME(6), UpdatedDate DATETIME(6), LogAddedDate DATETIME(6));";


                        MySqlCommand command3 = new MySqlCommand(query, connection2);
                        await command3.ExecuteNonQueryAsync();

                        await connection2.CloseAsync();
                    }
                    
                }
                catch (Exception error)
                {
                    Console.WriteLine("ERROR: "+error);
                }
                finally
                {
                    await connection1.CloseAsync();
                }
            }
        }


        // ** PROCEDURE OPERATIONS - BU IŞLEMLER ADO.NET ILE DE YAPILABILIRDI, ANCAK IŞLEMLER DAHA DA UZAYACAKTI !
        private static async Task SeedStoredProceduresForMsSQLAsync(DbContext dbContext)
        {
            // ** GET ALL CURRENCIES
            await dbContext.Database.ExecuteSqlRawAsync(@"CREATE OR ALTER PROCEDURE GetAllCurrencies
                                                        AS
                                                        BEGIN
                                                            SELECT * FROM dbo.Currencies;
                                                        END;");

            // ** GET CURRENCY BY ID
            await dbContext.Database.ExecuteSqlRawAsync(@"CREATE OR ALTER PROCEDURE GetCurrencyById(@CurrencyId INT)
                                                        AS
                                                        BEGIN
                                                            SELECT * FROM dbo.Currencies WHERE CurrencyId = @CurrencyId;
                                                        END;");

            // ** CREATE CURRENCY
            await dbContext.Database.ExecuteSqlRawAsync(@"CREATE OR ALTER PROCEDURE CreateCurrency(
                                                        @CurrencyCode NVARCHAR(30),
                                                        @Name NVARCHAR(100),
                                                        @Unit INT,
                                                        @ForexBuying FLOAT,
                                                        @ForexSelling FLOAT,
                                                        @BanknoteBuying FLOAT,
                                                        @BanknoteSelling FLOAT,
                                                        @CreatedDate DATETIME,
                                                        @UpdatedDate DATETIME)
                                                        AS
                                                        BEGIN
                                                            INSERT INTO dbo.Currencies(CurrencyCode, Name, Unit, ForexBuying, ForexSelling, BanknoteBuying, BanknoteSelling, CreatedDate, UpdatedDate)
                                                                                VALUES(@CurrencyCode, @Name, @Unit, @ForexBuying, @ForexSelling, @BanknoteBuying, @BanknoteSelling, @CreatedDate, @UpdatedDate);
                                                            SELECT * FROM dbo.Currencies WHERE CurrencyId = SCOPE_IDENTITY(); -- Eklenen Kaydın son Kimlik değerini 'SCOPE_IDENTITY()' ile alarak geriye döndürdüm.
                                                        END;");

            // ** UPDATE CURRENCY
            await dbContext.Database.ExecuteSqlRawAsync(@"CREATE OR ALTER PROCEDURE UpdateCurrency(
                                                        @CurrencyId INT,
                                                        @CurrencyCode NVARCHAR(30),
                                                        @Name NVARCHAR(100),
                                                        @Unit INT,
                                                        @ForexBuying FLOAT,
                                                        @ForexSelling FLOAT,
                                                        @BanknoteBuying FLOAT,
                                                        @BanknoteSelling FLOAT,
                                                        @UpdatedDate DATETIME)
                                                        AS
                                                        BEGIN
                                                            UPDATE dbo.Currencies SET CurrencyCode=@CurrencyCode, Name=@Name, Unit=@Unit, ForexBuying=@ForexBuying, ForexSelling=@ForexSelling, BanknoteBuying=@BanknoteBuying,
                                                            BanknoteSelling=@BanknoteSelling, UpdatedDate=@UpdatedDate WHERE CurrencyId = @CurrencyId;
                                                            SELECT * FROM dbo.Currencies WHERE CurrencyId = @CurrencyId;
                                                        END;");

            // ** DELETE CURRENCY
            await dbContext.Database.ExecuteSqlRawAsync(@"CREATE OR ALTER PROCEDURE DeleteCurrency(@CurrencyId INT)
                                                        AS
                                                        BEGIN
                                                            DELETE FROM dbo.Currencies WHERE CurrencyId = @CurrencyId;
                                                        END;");
        }

        private static async Task SeedStoredProceduresForMySQLAsync(DbContext dbContext)
        {
            // ** "DELIMITER" komutu MySQL veritabanı komutu olduğu için, EF Core içerisinde kullanmıyoruz, yoksa hata alırız !
                        
            // ** GET ALL CURRENCIES
            await dbContext.Database.ExecuteSqlRawAsync(@"CREATE PROCEDURE IF NOT EXISTS GetAllCurrencies()
                                                        BEGIN
                                                            SELECT * FROM dovizappdb.currencies;
                                                        END");

            // ** GET CURRENCY BY ID
            await dbContext.Database.ExecuteSqlRawAsync(@"CREATE PROCEDURE IF NOT EXISTS GetCurrencyById(IN p_currencyId INT)
                                                        BEGIN -- SQL kod bloğunun başladığını belirttik.
                                                            SELECT * FROM dovizappdb.currencies WHERE currencyId = p_currencyId; -- Normal SQL sorgumuzu yazdık.
                                                        END");

            // ** CREATE CURRENCY
            await dbContext.Database.ExecuteSqlRawAsync(@"CREATE PROCEDURE IF NOT EXISTS CreateCurrency(
                                                        IN p_currencyCode VARCHAR(30),
                                                        IN p_name VARCHAR(45),
                                                        IN p_unit INT,
                                                        IN p_forexBuying FLOAT,
                                                        IN p_forexSelling FLOAT,
                                                        IN p_banknoteBuying FLOAT,
                                                        IN p_banknoteSelling FLOAT,
                                                        IN p_createdDate DATETIME,
                                                        IN p_updatedDate DATETIME)
                                                        BEGIN
                                                            INSERT INTO dovizappdb.currencies(currencyCode, name, unit, forexBuying, forexSelling, banknoteBuying, banknoteSelling, createdDate, updatedDate)
                                                                                    VALUES(p_currencyCode, p_name, p_unit, p_forexBuying, p_forexSelling, p_banknoteBuying, p_banknoteSelling, p_createdDate, p_updatedDate);
                                                            SELECT * FROM dovizappdb.currencies WHERE currencyId = LAST_INSERT_ID();
                                                        END");

            // ** UPDATE CURRENCY
            await dbContext.Database.ExecuteSqlRawAsync(@"CREATE PROCEDURE IF NOT EXISTS UpdateCurrency(
                                                        IN p_currencyId INT,
                                                        IN p_currencyCode VARCHAR(30),
                                                        IN p_name VARCHAR(45),
                                                        IN p_unit INT,
                                                        IN p_forexBuying FLOAT,
                                                        IN p_forexSelling FLOAT,
                                                        IN p_banknoteBuying FLOAT,
                                                        IN p_banknoteSelling FLOAT,
                                                        IN p_updatedDate DATETIME)
                                                        BEGIN
                                                            UPDATE dovizappdb.currencies SET currencyCode=p_currencyCode, name=p_name, unit=p_unit, forexBuying=p_forexBuying, forexSelling=p_forexSelling,
                                                            banknoteBuying=p_banknoteBuying, banknoteSelling=p_banknoteSelling, updatedDate=p_updatedDate WHERE currencyId=p_currencyId;
                                                            SELECT * FROM dovizappdb.currencies WHERE currencyId = p_currencyId;

                                                        END");

            // ** DELETE CURRENCY
            await dbContext.Database.ExecuteSqlRawAsync(@"CREATE PROCEDURE IF NOT EXISTS DeleteCurrency (IN p_currencyId INT)
                                                        BEGIN
                                                            DELETE FROM dovizappdb.currencies WHERE currencyId = p_currencyId;
                                                        END");
        }
    
    }
}