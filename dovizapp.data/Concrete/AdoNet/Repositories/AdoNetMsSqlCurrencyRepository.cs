using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using dovizapp.data.Abstract;
using dovizapp.entity;
using dovizapp.shared.Utilities.Extensions;
using Microsoft.Data.SqlClient;

namespace dovizapp.data.Concrete.AdoNet.Repositories
{
    public class AdoNetMsSqlCurrencyRepository : ICurrencyRepository
    {
        private readonly string _connectionString;
        public AdoNetMsSqlCurrencyRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private SqlConnection GetMsSqlConnection()
        {            
            return new SqlConnection(_connectionString);
        }        
        // ** "int.parse()" or "reader.GetInt32()": "int.parse", eğer dönüşüm işlemi başarısız olursa geriye bir "FormatException" hatası fırlatır.
        // ** "reader.GetInt32()": Veritabanından gelen veriyi doğrudan "int" türünde alır ve bir dönüşüm hatası fırlatmaz.


        public async Task<List<Currency>> GetAllUsingProcedureAsync()
        {
            List<Currency> currencies = new List<Currency>();
            using (var connection = GetMsSqlConnection())
            {
                try
                {
                    await connection.OpenAsync();

                    // SqlCommand command = new SqlCommand("SELECT * FROM [currencies]", connection);
                    SqlCommand command = new SqlCommand("GetAllCurrencies", connection); // Stored Procedure ismini yazdık.
                    command.CommandType = CommandType.StoredProcedure; // Stored Procedure çalıştıracağımız için "StoredProcedure" tipini belirtmemiz gerekiyor. Çünkü Default olarak Text'dir.
                    
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        // ** MAPPING OPERATION !
                        currencies.Add(ReaderExtensions.ReaderToCurrency<Currency>(reader)); // Kod fazlalığı olduğu için "MAPPING" işlemini method içerisine aldım.
                    }

                    await reader.CloseAsync(); // Burada, "SqlDataReader" nesnesinin kapatılmasını ve kaynakların serbest bırakarak bellek kullanımını azaltıyoruz. Böylece, bellek kullanımını azalır ve gereksiz kaynakların uzun süreli olarak tutulmasını önler.
                }
                catch (Exception error)
                {
                    Console.WriteLine("ERROR: "+error);
                }
                finally
                {
                    await connection.CloseAsync();
                }
                
                return currencies;
            }
        }

        public async Task<Currency> GetByIdUsingProcedureAsync(int id)
        {
            Currency currency = null;
            using (var connection = GetMsSqlConnection())
            {
                try
                {
                    await connection.OpenAsync();

                    // SqlCommand command = new SqlCommand("SELECT * FROM [currencies] WHERE CurrencyId = @currencyId", connection);
                    SqlCommand command = new SqlCommand("GetCurrencyById", connection); // Stored Procedure ismini yazdık.
                    command.CommandType = CommandType.StoredProcedure; // Stored Procedure çalıştıracağımız için "StoredProcedure" tipini belirtmemiz gerekiyor. Çünkü Default olarak Text'dir.
                    command.Parameters.AddWithValue("@currencyId", id);

                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    await reader.ReadAsync(); // Bir sonraki satırı okuduk.
                    if (reader.HasRows) // Sorguda en az bir satır içerip içermediğini kontrol ettik.
                    {
                        // ** MAPPING OPERATION !
                        currency = ReaderExtensions.ReaderToCurrency<Currency>(reader); // Kod fazlalığı olduğu için "MAPPING" işlemini method içerisine aldım.
                    }

                    await reader.CloseAsync(); // Burada, "SqlDataReader" nesnesinin kapatılmasını ve kaynakların serbest bırakarak bellek kullanımını azaltıyoruz. Böylece, bellek kullanımını azalır ve gereksiz kaynakların uzun süreli olarak tutulmasını önler.
                }
                catch (Exception error)
                {
                    Console.WriteLine("ERROR: "+error);
                }
                finally
                {
                    await connection.CloseAsync();
                } 

                return currency;
            }
        }

        public async Task<Currency> CreateUsingProcedureAsync(Currency entity)
        {
            Currency currency = null;
            using (var connection = GetMsSqlConnection())
            {
                try
                {
                    await connection.OpenAsync();

                    // SqlCommand command = new SqlCommand("INSERT INTO [currencies] (Name, Unit, ForexBuying, ForexSelling, BanknoteBuying, BanknoteSelling, CreatedDate, UpdatedDate) VALUES(@name, @unit, @forexBuying, @forexSelling, @banknoteBuying, @banknoteSelling, @createdDate, @updatedDate)", connection);
                    SqlCommand command = new SqlCommand("CreateCurrency", connection); // Stored Procedure ismini yazdık.
                    command.CommandType = CommandType.StoredProcedure; // Stored Procedure çalıştıracağımız için "StoredProcedure" tipini belirtmemiz gerekiyor. Çünkü Default olarak Text'dir.
                    
                    command.Parameters.AddWithValue("@currencyCode", entity.CurrencyCode);
                    command.Parameters.AddWithValue("@name", entity.Name);
                    command.Parameters.AddWithValue("@unit", entity.Unit);
                    command.Parameters.AddWithValue("@forexBuying", entity.ForexBuying);
                    command.Parameters.AddWithValue("@forexSelling", entity.ForexSelling);
                    command.Parameters.AddWithValue("@banknoteBuying", entity.BanknoteBuying);
                    command.Parameters.AddWithValue("@banknoteSelling", entity.BanknoteSelling);
                    command.Parameters.AddWithValue("@createdDate", DateTime.Now);
                    command.Parameters.AddWithValue("@updatedDate", DateTime.Now);

                    //await command.ExecuteNonQueryAsync(); // Sorguyu çalıştırır ve etkilenen satır sayısını döndürür.

                    // ** TAKING THE CURRENCY WHICH IS CREATED !
                    var reader = await command.ExecuteReaderAsync(); // "ExecuteNonQueryAsync" kullanmadan veritabanı işlemlerini gerçekleştirdim.
                    if (await reader.ReadAsync())
                    {
                        currency = ReaderExtensions.ReaderToCurrency<Currency>(reader);
                    }

                    await reader.CloseAsync();
                }
                catch (Exception error)
                {
                    Console.WriteLine("ERROR: "+error);
                }
                finally
                {
                    await connection.CloseAsync();
                }

                return currency;
            }
        }

        public async Task<Currency> UpdateUsingProcedureAsync(Currency entity)
        {
            Currency currency = null;
            using (var connection = GetMsSqlConnection())
            {
                try
                {
                    await connection.OpenAsync();

                    // ** UPDATE OPERATION !
                    // SqlCommand command = new SqlCommand("UPDATE [currencies] SET name=@name, unit=@unit, forexBuying=@forexBuying, forexSelling=@forexSelling, banknoteBuying=@banknoteBuying, banknoteSelling=@banknoteSelling, updatedDate=@updatedDate WHERE currencyId = @currencyId", connection);
                    SqlCommand command = new SqlCommand("UpdateCurrency", connection); // Stored Procedure ismini yazdık.
                    command.CommandType = CommandType.StoredProcedure; // Stored Procedure çalıştıracağımız için "StoredProcedure" tipini belirtmemiz gerekiyor. Çünkü Default olarak Text'dir.
                    
                    command.Parameters.AddWithValue("@currencyId", entity.CurrencyId);
                    command.Parameters.AddWithValue("@currencyCode", entity.CurrencyCode);
                    command.Parameters.AddWithValue("@name", entity.Name);
                    command.Parameters.AddWithValue("@unit", entity.Unit);
                    command.Parameters.AddWithValue("@forexBuying", entity.ForexBuying);
                    command.Parameters.AddWithValue("@forexSelling", entity.ForexSelling);
                    command.Parameters.AddWithValue("@banknoteBuying", entity.BanknoteBuying);
                    command.Parameters.AddWithValue("@banknoteSelling", entity.BanknoteSelling);
                    command.Parameters.AddWithValue("@updatedDate", DateTime.Now);

                    //await command.ExecuteNonQueryAsync(); // Sorguyu çalıştırır ve etkilenen satır sayısını döndürür.

                    // ** TAKING THE CURRENCY WHICH IS UPDATED !
                    SqlDataReader reader = await command.ExecuteReaderAsync(); // "ExecuteNonQueryAsync" kullanmadan veritabanı işlemlerini gerçekleştirdim.
                    if (await reader.ReadAsync())
                    {
                        currency = ReaderExtensions.ReaderToCurrency<Currency>(reader);
                    }

                    await reader.CloseAsync();

                }
                catch (Exception error)
                {
                    Console.WriteLine("ERROR: "+error);
                }
                finally
                {
                    await connection.CloseAsync();
                }
                
                return currency;
            }
        }

        public async Task DeleteUsingProcedureAsync(Currency entity)
        {
            using (var connection = GetMsSqlConnection())
            {
                try
                {
                    await connection.OpenAsync();

                    // SqlCommand command = new SqlCommand("DELETE FROM [currencies] WHERE currencyId=@currencyId", connection);
                    SqlCommand command = new SqlCommand("DeleteCurrency", connection); // Stored Procedure ismini yazdık.
                    command.CommandType = CommandType.StoredProcedure; // Stored Procedure çalıştıracağımız için "StoredProcedure" tipini belirtmemiz gerekiyor. Çünkü Default olarak Text'dir.

                    command.Parameters.AddWithValue("@currencyId", entity.CurrencyId);

                    await command.ExecuteNonQueryAsync(); // Sorguyu çalıştırır ve etkilenen satır sayısını döndürür.
                }
                catch (Exception error)
                {
                    Console.WriteLine("ERROR: "+error);
                }
                finally
                {
                    await connection.CloseAsync();
                }
            }
        }



        public async Task DeleteAllCurrenciesAsync()
        {
            using (var connection = GetMsSqlConnection())
            {
                try
                {
                    await connection.OpenAsync();

                    SqlCommand command = new SqlCommand("DELETE FROM [currencies]", connection);
                    await command.ExecuteNonQueryAsync();
                                        
                }
                catch (Exception error)
                {
                    Console.WriteLine("ERROR: "+error);
                }
                finally
                {
                    await connection.CloseAsync();
                }
            }
        }
    
        public async Task<Currency> GetLatestCurrencyAsync()
        {
            Currency currency = null;
            using (var connection = GetMsSqlConnection())
            {
                try
                {
                    await connection.OpenAsync();

                    SqlCommand command = new SqlCommand("SELECT TOP 1 * FROM [currencies] ORDER BY updatedDate DESC", connection);
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (await reader.ReadAsync())
                    {
                        Console.WriteLine("BEFORE: "+reader.GetDateTime(reader.GetOrdinal("updatedDate")));
                        currency = ReaderExtensions.ReaderToCurrency<Currency>(reader);
                        Console.WriteLine("AFTER: "+currency.UpdatedDate);
                    }

                    await reader.CloseAsync();

                }
                catch (Exception error)
                {
                    Console.WriteLine("ERROR: "+error);
                }
                finally
                {
                    await connection.CloseAsync();
                }

                return currency;
            }
        }
        
        public async Task TransferAllCurrenciesToLogTableAsync()
        {
            using (var connection1 = GetMsSqlConnection())
            {
                try
                {
                    await connection1.OpenAsync();

                    SqlCommand command1 = new SqlCommand("GetAllCurrencies", connection1); // Stored Procedure ismini yazdık.
                    command1.CommandType = CommandType.StoredProcedure; // Stored Procedure çalıştıracağımız için "StoredProcedure" tipini belirtmemiz gerekiyor. Çünkü Default olarak Text'dir.

                    SqlDataReader reader = await command1.ExecuteReaderAsync();

                    using (var connection2 = GetMsSqlConnection()) // Reader nesnemiz ile, Insert methodumuz, işlem yaptığı sırada çarpıştığı için başka bir "Connection" açtık !
                    {
                        await connection2.OpenAsync();
                        while (await reader.ReadAsync())
                        {
                            SqlCommand command2 = new SqlCommand("INSERT INTO [currencyLogs] (CurrencyId, CurrencyCode, Name, Unit, ForexBuying, ForexSelling, BanknoteBuying, BanknoteSelling, CreatedDate, UpdatedDate, LogAddedDate) VALUES(@currencyId, @currencyCode, @name, @unit, @forexBuying, @forexSelling, @banknoteBuying, @banknoteSelling, @createdDate, @updatedDate, @logAddedDate)", connection2);
                            command2.Parameters.AddWithValue("@currencyId", reader.GetInt32(reader.GetOrdinal("currencyId")));
                            command2.Parameters.AddWithValue("@currencyCode", reader.GetString(reader.GetOrdinal("currencyCode")));
                            command2.Parameters.AddWithValue("@name", reader.GetString(reader.GetOrdinal("name")));
                            command2.Parameters.AddWithValue("@unit", reader.GetInt32(reader.GetOrdinal("unit")));
                            command2.Parameters.AddWithValue("@forexBuying", reader.GetDouble(reader.GetOrdinal("forexBuying")));
                            command2.Parameters.AddWithValue("@forexSelling", reader.GetDouble(reader.GetOrdinal("forexSelling")));
                            command2.Parameters.AddWithValue("@banknoteBuying", reader.GetDouble(reader.GetOrdinal("banknoteBuying")));
                            command2.Parameters.AddWithValue("@banknoteSelling", reader.GetDouble(reader.GetOrdinal("banknoteSelling")));
                            command2.Parameters.AddWithValue("@createdDate", reader.GetDateTime(reader.GetOrdinal("createdDate")));
                            command2.Parameters.AddWithValue("@updatedDate", reader.GetDateTime(reader.GetOrdinal("updatedDate")));
                            command2.Parameters.AddWithValue("@logAddedDate", DateTime.Now);

                            await command2.ExecuteNonQueryAsync();
                        }
                    }

                    await reader.CloseAsync();
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
        
        public async Task UpdateAllCurrenciesAsync(List<Currency> entities)
        {
            using (var connection1 = GetMsSqlConnection())
            {
                try
                {
                    await connection1.OpenAsync();

                    foreach (var currency in entities)
                    {
                        SqlCommand command1 = new SqlCommand("SELECT * FROM [currencies] WHERE currencyCode=@currencyCode", connection1);
                        command1.Parameters.AddWithValue("@currencyCode", currency.CurrencyCode);

                        SqlDataReader reader = await command1.ExecuteReaderAsync();                        

                        using (var connection2 = GetMsSqlConnection())
                        {
                            await connection2.OpenAsync();
                            SqlCommand command2 = null;

                            if (await reader.ReadAsync())
                            {
                                command2 = new SqlCommand("UPDATE [currencies] SET name=@name, unit=@unit, forexBuying=@forexBuying, forexSelling=@forexSelling, banknoteBuying=@banknoteBuying, banknoteSelling=@banknoteSelling, updatedDate=@updatedDate WHERE currencyCode=@currencyCode ", connection2);
                                
                                command2.Parameters.AddWithValue("@currencyCode", currency.CurrencyCode);
                                command2.Parameters.AddWithValue("@name", currency.Name);
                                command2.Parameters.AddWithValue("@unit", currency.Unit);
                                command2.Parameters.AddWithValue("@forexBuying", currency.ForexBuying);
                                command2.Parameters.AddWithValue("@forexSelling", currency.ForexSelling);
                                command2.Parameters.AddWithValue("@banknoteBuying", currency.BanknoteBuying);
                                command2.Parameters.AddWithValue("@banknoteSelling", currency.BanknoteSelling);
                                command2.Parameters.AddWithValue("@updatedDate", DateTime.Now);

                            } else {
                                command2 = new SqlCommand("CreateCurrency", connection2); // Stored Procedure ismini yazdık.
                                command2.CommandType = CommandType.StoredProcedure; // Stored Procedure çalıştıracağımız için "StoredProcedure" tipini belirtmemiz gerekiyor. Çünkü Default olarak Text'dir.
                                
                                command2.Parameters.AddWithValue("@currencyCode", currency.CurrencyCode);
                                command2.Parameters.AddWithValue("@name", currency.Name);
                                command2.Parameters.AddWithValue("@unit", currency.Unit);
                                command2.Parameters.AddWithValue("@forexBuying", currency.ForexBuying);
                                command2.Parameters.AddWithValue("@forexSelling", currency.ForexSelling);
                                command2.Parameters.AddWithValue("@banknoteBuying", currency.BanknoteBuying);
                                command2.Parameters.AddWithValue("@banknoteSelling", currency.BanknoteSelling);
                                command2.Parameters.AddWithValue("@createdDate", DateTime.Now);
                                command2.Parameters.AddWithValue("@updatedDate", DateTime.Now);
                            }

                            await command2.ExecuteNonQueryAsync();
                        }

                        await reader.CloseAsync(); // * Reader' kapatılmazsa, programda birden fazla reader çalışacağı için hata verecek ve program çökecektir !
                    }
                }
                catch (Exception error)
                {
                    Console.WriteLine("ERROR:"+error);
                }
                finally
                {
                    await connection1.CloseAsync();
                }
            }
        }


        // public Currency ReaderToCurrency(SqlDataReader reader)
        // {        
        //     return new Currency() {
        //         CurrencyId = reader.GetInt32(reader.GetOrdinal("currencyId")),
        //         CurrencyCode = reader.GetString(reader.GetOrdinal("currencyCode")),
        //         Name = reader.GetString(reader.GetOrdinal("name")),
        //         Unit = reader.GetInt32(reader.GetOrdinal("unit")),
        //         ForexBuying = reader.GetDouble(reader.GetOrdinal("forexBuying")),
        //         ForexSelling = reader.GetDouble(reader.GetOrdinal("forexSelling")),
        //         BanknoteBuying = reader.GetDouble(reader.GetOrdinal("banknoteBuying")),
        //         BanknoteSelling = reader.GetDouble(reader.GetOrdinal("banknoteSelling")),
        //         CreatedDate = reader.GetDateTime(reader.GetOrdinal("createdDate")),
        //         UpdatedDate = reader.GetDateTime(reader.GetOrdinal("updatedDate"))
        //     };
        // }
        

        
        public async Task<Currency> CreateAsync(Currency entity)
        {
            await Task.CompletedTask; // Uyarı kapatılmak için yapıldı.
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(Currency entity)
        {
            await Task.CompletedTask; // Uyarı kapatılmak için yapıldı.
            throw new NotImplementedException();
        }

        public async Task<List<Currency>> GetAllAsync()
        {
            await Task.CompletedTask; // Uyarı kapatılmak için yapıldı.
            throw new NotImplementedException();
        }

        public async Task<Currency> GetByIdAsync(int id)
        {
            await Task.CompletedTask; // Uyarı kapatılmak için yapıldı.
            throw new NotImplementedException();
        }

        public async Task<Currency> UpdateAsync(Currency entity)
        {
            await Task.CompletedTask; // Uyarı kapatılmak için yapıldı.
            throw new NotImplementedException();
        }

    }
}