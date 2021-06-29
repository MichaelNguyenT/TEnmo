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

        public string GetBalance()
        {
            string returnBalance = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT balance " +
                        "FROM accounts WHERE user_id = @userID", conn);
                    cmd.Parameters.AddWithValue("@userID", userID);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        decimal balance = Convert.ToDecimal(reader);
                        returnBalance = "Your current account balance is: " + balance;
                    }
                }
            }
            catch(SqlException)
            {
                throw;
            }
            return returnBalance;
        }


    }
}
