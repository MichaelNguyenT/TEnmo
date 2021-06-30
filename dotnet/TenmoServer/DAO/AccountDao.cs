using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace TenmoServer.DAO
{
    public class AccountDao : IAccountDao
    {
        private readonly string connectionString;


        public AccountDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public decimal GetBalance(int userId)
        {
            decimal balance = 0.0M;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT balance " +
                        "FROM accounts WHERE user_id = @userID", conn);
                    cmd.Parameters.AddWithValue("@userID", userId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        balance = Convert.ToDecimal(reader["balance"]);
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            return balance;
        }
    }
}
