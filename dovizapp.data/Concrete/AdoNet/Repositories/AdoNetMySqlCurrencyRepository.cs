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
using MySqlConnector;

namespace dovizapp.data.Concrete.AdoNet.Repositories
{
    public class AdoNetMySqlCurrencyRepository : ICurrencyRepository
    {
        private readonly string _connectionString;
        public AdoNetMySqlCurrencyRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private MySqlConnection GetMySqlConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        public async Task<List<Currency>> GetAllUsingProcedureAsync()
        {
            List<Currency> currencies = new List<Currency>();
            using (var connection = GetMySqlConnection())
            {
                try
                {
                    await connection.OpenAsync();

                    // MySqlCommand command = new MySqlCommand("SELECT * FROM currencies", connection);
                    MySqlCommand command = new MySqlCommand("GetAllCurrencies", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    DbDataReader reader = await command.ExecuteReaderAsync(); // Methodumuz "Async" olduğu için "MySqlDataReader" yerine "DbDataReader" kullandım.

                    while (await reader.ReadAsync())
                    {
                        currencies.Add(ReaderExtensions.ReaderToCurrency<Currency>(reader));
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

                return currencies;
            }    
        }

        public async Task<Currency> GetByIdUsingProcedureAsync(int id)
        {
            Currency currency = null;
            using (var connection = GetMySqlConnection())
            {
                try
                {
                    await connection.OpenAsync();

                    // MySqlCommand command = new MySqlCommand("SELECT * FROM currencies WHERE currencyId=@currencyId", connection);
                    MySqlCommand command = new MySqlCommand("GetCurrencyById", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_currencyId", id);

                    DbDataReader reader = await command.ExecuteReaderAsync(); // Methodumuz "Async" olduğu için "MySqlDataReader" yerine "DbDataReader" kullandım.

                    await reader.ReadAsync(); // Bir sonraki satırı okuduk.
                    if (reader.HasRows) // Sorguda en az bir satır içerip içermediğini kontrol ettik.
                    {
                        // ** MAPPING OPERATION !
                        currency = ReaderExtensions.ReaderToCurrency<Currency>(reader); // Kod fazlalığı olduğu için "MAPPING" işlemini method içerisine aldım.
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

        public async Task<Currency> CreateUsingProcedureAsync(Currency entity)
        {
            using (var connection = GetMySqlConnection())
            {
                try
                {
                    await connection.OpenAsync();

                    // MySqlCommand command = new MySqlCommand("INSERT INTO currencies (currencyCode, name, unit, forexBuying, forexSelling, banknoteBuying, banknoteSelling, createdDate, updatedDate) VALUES(@currencyCode, @name, @unit, @forexBuying, @forexSelling, @banknoteBuying, @banknoteSelling, @createdDate, @updatedDate)", connection);
                    MySqlCommand command = new MySqlCommand("CreateCurrency", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@p_currencyCode", entity.CurrencyCode);
                    command.Parameters.AddWithValue("@p_name", entity.Name);
                    command.Parameters.AddWithValue("@p_unit", entity.Unit);
                    command.Parameters.AddWithValue("@p_forexBuying", entity.ForexBuying);
                    command.Parameters.AddWithValue("@p_forexSelling", entity.ForexSelling);
                    command.Parameters.AddWithValue("@p_banknoteBuying", entity.BanknoteBuying);
                    command.Parameters.AddWithValue("@p_banknoteSelling", entity.BanknoteSelling);
                    command.Parameters.AddWithValue("@p_createdDate", DateTime.Now);
                    command.Parameters.AddWithValue("@p_updatedDate", DateTime.Now);

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

                return entity;
            }
        }

        public async Task<Currency> UpdateUsingProcedureAsync(Currency entity)
        {
            Currency currency = null;
            using (var connection = GetMySqlConnection())
            {
                try
                {
                    await connection.OpenAsync();

                    // ** UPDATE OPERATION !
                    // MySqlCommand command = new MySqlCommand("UPDATE currencies SET currencyCode=@currencyCode, name=@name, unit=@unit, forexBuying=@forexBuying, forexSelling=@forexSelling, banknoteBuying=@banknoteBuying, banknoteSelling=@banknoteSelling, updatedDate=@updatedDate WHERE currencyId=@currencyId", connection);
                    MySqlCommand command = new MySqlCommand("UpdateCurrency", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@p_currencyId", entity.CurrencyId);
                    command.Parameters.AddWithValue("@p_currencyCode", entity.CurrencyCode);
                    command.Parameters.AddWithValue("@p_name", entity.Name);
                    command.Parameters.AddWithValue("@p_unit", entity.Unit);
                    command.Parameters.AddWithValue("@p_forexBuying", entity.ForexBuying);
                    command.Parameters.AddWithValue("@p_forexSelling", entity.ForexSelling);
                    command.Parameters.AddWithValue("@p_banknoteBuying", entity.BanknoteBuying);
                    command.Parameters.AddWithValue("@p_banknoteSelling", entity.BanknoteSelling);
                    command.Parameters.AddWithValue("@p_updatedDate", entity.UpdatedDate);

                    await command.ExecuteNonQueryAsync(); // Sorguyu çalıştırır ve etkilenen satır sayısını döndürür.


                    // ** TAKING THE CURRENCY WHICH IS UPDATED !
                    MySqlCommand getCurrencycommand = new MySqlCommand("SELECT * FROM currencies WHERE currencyId=@currencyId");
                    getCurrencycommand.Parameters.AddWithValue("@currencyId", entity.CurrencyId);

                    DbDataReader reader = await command.ExecuteReaderAsync(); // Methodumuz "Async" olduğu için "MySqlDataReader" yerine "DbDataReader" kullandım.
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
            using (var connection = GetMySqlConnection())
            {
                try
                {
                    await connection.OpenAsync();

                    // MySqlCommand command = new MySqlCommand("DELETE FROM currencies WHERE currencyId=@currencyId", connection);
                    MySqlCommand command = new MySqlCommand("DeleteCurrency", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_currencyId", entity.CurrencyId);

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
            using (var connection = GetMySqlConnection())
            {
                try
                {
                    await connection.OpenAsync();
                    
                    MySqlCommand command = new MySqlCommand("SELECT * FROM currencies ORDER BY updatedDate DESC LIMIT 1", connection);
                    DbDataReader reader = await command.ExecuteReaderAsync();

                    if (await reader.ReadAsync())
                    {
                        currency = ReaderExtensions.ReaderToCurrency<Currency>(reader);
                    }

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

            return currency;
        }

        public async Task DeleteAllCurrenciesAsync()
        {
            using (var connection = GetMySqlConnection())
            {
                try
                {
                    await connection.OpenAsync();

                    MySqlCommand command = new MySqlCommand("DELETE FROM currencies", connection);
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
        

        public async Task TransferAllCurrenciesToLogTableAsync()
        {
            using (var connection1 = GetMySqlConnection())
            {
                try
                {
                    await connection1.OpenAsync();

                    MySqlCommand command1 = new MySqlCommand("SELECT * FROM currencies", connection1); 

                    DbDataReader reader = await command1.ExecuteReaderAsync();

                    using (var connection2 = GetMySqlConnection()) // Reader nesnemiz ile, Insert methodumuz, işlem yaptığı sırada çarpıştığı için başka bir "Connection" açtık !
                    {
                        await connection2.OpenAsync();
                        while (await reader.ReadAsync())
                        {
                            MySqlCommand command2 = new MySqlCommand("INSERT INTO currencyLogs (CurrencyId, CurrencyCode, Name, Unit, ForexBuying, ForexSelling, BanknoteBuying, BanknoteSelling, CreatedDate, UpdatedDate, LogAddedDate) VALUES(@currencyId, @currencyCode, @name, @unit, @forexBuying, @forexSelling, @banknoteBuying, @banknoteSelling, @createdDate, @updatedDate, @logAddedDate)", connection2);
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
            using (var connection1 = GetMySqlConnection())
            {
                try
                {
                    await connection1.OpenAsync();

                    foreach (var currency in entities)
                    {
                        MySqlCommand command1 = new MySqlCommand("SELECT * FROM currencies WHERE currencyCode=@currencyCode", connection1);
                        command1.Parameters.AddWithValue("@currencyCode", currency.CurrencyCode);

                        DbDataReader reader = await command1.ExecuteReaderAsync();

                        using (var connection2 = GetMySqlConnection())
                        {
                            await connection2.OpenAsync();
                            MySqlCommand command2 = null;

                            if (await reader.ReadAsync())
                            {
                                command2 = new MySqlCommand("UPDATE currencies SET name=@name, unit=@unit, forexBuying=@forexBuying, forexSelling=@forexSelling, banknoteBuying=@banknoteBuying, banknoteSelling=@banknoteSelling, updatedDate=@updatedDate WHERE currencyCode=@currencyCode", connection2);
                                
                                command2.Parameters.AddWithValue("@currencyCode", currency.CurrencyCode);
                                command2.Parameters.AddWithValue("@name", currency.Name);
                                command2.Parameters.AddWithValue("@unit", currency.Unit);
                                command2.Parameters.AddWithValue("@forexBuying", currency.ForexBuying);
                                command2.Parameters.AddWithValue("@forexSelling", currency.ForexSelling);
                                command2.Parameters.AddWithValue("@banknoteBuying", currency.BanknoteBuying);
                                command2.Parameters.AddWithValue("@banknoteSelling", currency.BanknoteSelling);
                                command2.Parameters.AddWithValue("@updatedDate", DateTime.Now);

                            } else {
                                command2 = new MySqlCommand("INSERT INTO currencies (currencyCode, name, unit, forexBuying, forexSelling, banknoteBuying, banknoteSelling, createdDate, updatedDate) VALUES(@currencyCode, @name, @unit, @forexBuying, @forexSelling, @banknoteBuying, @banknoteSelling, @createdDate, @updatedDate)", connection2);
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

                        await reader.CloseAsync(); // * Reader'ı kapatmazsam, programda birden fazla reader çalışacağı için hata verecek ve program çökecektir !
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



        public async Task<Currency> CreateAsync(Currency entity)
        {
            await Task.CompletedTask; // Uyarıyı kapatmak için yaptım.
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(Currency entity)
        {
            await Task.CompletedTask; // Uyarıyı kapatmak için yaptım.
            throw new NotImplementedException();
        }

        public async Task<List<Currency>> GetAllAsync()
        {
            await Task.CompletedTask; // Uyarıyı kapatmak için yaptım.
            throw new NotImplementedException();     
        }

        public async Task<Currency> GetByIdAsync(int id)
        {
            await Task.CompletedTask; // Uyarıyı kapatmak için yaptım.
            throw new NotImplementedException();
        }

        public async Task<Currency> UpdateAsync(Currency entity)
        {
            await Task.CompletedTask; // Uyarıyı kapatmak için yaptım.
            throw new NotImplementedException();
        }

    }
}