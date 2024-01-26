using Microsoft.Data.Sqlite;
using SecurityDemo.Models;
using System.Collections.Generic;
using System.Data.SQLite;
using Microsoft.Extensions.Configuration;

namespace SecurityDemo.Repositories
{
    public class SqlDbRepository
    {
        private readonly IConfiguration _configuration;

        public SqlDbRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public List<string> GetCities(out string message)
        {
            message = string.Empty;
            List<string> cities = new List<string>();
            string sql = "SELECT * FROM Cities;";

            try
            {
                string connectionString = _configuration["ConnectionStrings:DefaultConnection"];
                using (SqliteConnection connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    using (SqliteCommand command = new SqliteCommand(sql, connection))
                    {
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cities.Add($"{reader.GetInt32(0)},{reader.GetString(1)}");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                message = $"Error retrieving cities: {e.Message}";
            }
            if (cities.Count() == 0)
            {
                message = "No cities.";
            }
            return cities;
        }

        public string GetCityName(string cityId)
        {
            string cityName = string.Empty;

            if(!IsValidId(cityId))
            {
                string errorMessage = "Invalid city id";
                return cityName;
            }

            string sql = "SELECT CityName FROM Cities WHERE cityId = @CityId;";

            try
            {
                string connectionString = _configuration["ConnectionStrings:DefaultConnection"];
                using (SqliteConnection connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    using (SqliteCommand command = new SqliteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@CityId", cityId);
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cityName = reader.GetString(0);
                            }
                        }
                    }
                }
            }
            catch (Exception e) 
            {
                string errorMessage = $"Error getting city name: {e.Message}";
            }
            return cityName;
        }

        public List<string> GetBuildingsInCity(string cityId)
        {
            if(!IsValidId(cityId))
            {
                string errorMessage = "Invalid city id";
                return new List<string>();
            }

            List<string> list = new List<string>();

            try
            {
                string connectionString = _configuration["ConnectionStrings:DefaultConnection"];
                using (SqliteConnection connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT Buildings.buildingId, Buildings.name, Rooms.name, Rooms.capacity " +
                        "FROM Buildings INNER JOIN Rooms ON Buildings.buildingId = Rooms.buildingId " +
                        "WHERE Buildings.cityId = @CityId;";
                    using (SqliteCommand command = new SqliteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@CityId", cityId);
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add($"{reader.GetInt32(0)},{reader.GetString(1)}," +
                                    $"{reader.GetString(2)},{reader.GetInt32(3)}");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string errorMessage = $"Error getting buildings in city: {e.Message}";
            }
            return list;
        }

        public List<string> GetRegisteredUsers()
        {
            List<string> list = new List<string>();

            try
            {
                string connectionString = _configuration["ConnectionStrings:DefaultConnection"];

                using (SqliteConnection connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT AspNetUsers.Id, AspNetUsers.UserName, AspNetUserRoles.RoleId " +
                        "FROM AspNetUsers INNER JOIN AspNetUserRoles ON AspNetUsers.Id = " +
                        "AspNetUserRoles.UserId INNER JOIN AspNetRolres ON AspNetRoles.RoleId = AspNetRoles.Id;";
                    using (SqliteCommand command = new SqliteCommand(sql, connection))
                    {
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add($"{reader.GetString(0)},{reader.GetString(1)}," +
                                        $"{reader.GetString(2)}");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string errorMessage = $"Error getting registered users: {e.Message}";
            }
            return list;
        }

        public List<ProductVM> GetProducts()
        {
            List<ProductVM> products = new List<ProductVM>();

            try
            {
                string connectionString = _configuration["ConnectionStrings:DefaultConnection"];

                using (SqliteConnection connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM Products";

                    using (SqliteCommand command = new SqliteCommand(sql, connection))
                    {
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                products.Add(new ProductVM
                                {
                                    ProdName = (string)reader["prodName"],
                                    ProdID = (string)reader["prodID"],
                                    Price = (double)reader["price"]
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string errorMessage = $"Error getting products: {e.Message}";
            }
            return products;
        }

        public ProductVM GetProduct(string productID)
        {
            if (!IsValidId(productID))
            {
                string errorMessaage = "Invalid product ID";
                return new ProductVM();
            }

            ProductVM productVM = new ProductVM();

            try
            {
                string connectionString = _configuration["ConnectionStrings:DefaultConnection"];

                using (SqliteConnection connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM Products WHERE prodID=@ProductID";

                    using (SqliteCommand command = new SqliteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ProductID", productID);

                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                productVM = new ProductVM
                                {
                                    ProdName = (string)reader["prodName"],
                                    ProdID = (string)reader["prodID"],
                                    Price = (double)reader["price"]
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string errorMessage = $"Error retrieving product: {e.Message}";
            }
            return productVM;
        }

        private bool IsValidId(string input)
        {
            return !string.IsNullOrEmpty(input) 
                && int.TryParse(input, out int parsedId) 
                && (parsedId > 0);
        }
    }
}
