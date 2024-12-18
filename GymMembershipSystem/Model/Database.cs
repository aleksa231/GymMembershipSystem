using System;
using MySqlConnector;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace GymMembershipSystem.Model
{
    public class Database
    {
        private readonly string connectionString;

        public Database()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) 
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) 
                .Build();

            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}


