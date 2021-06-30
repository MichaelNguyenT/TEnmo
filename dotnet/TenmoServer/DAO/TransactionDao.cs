using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TenmoServer.Models;
using System.Data.SqlClient;

namespace TenmoServer.DAO
{
    public class TransactionDao : ITransactionDao
    {
        private readonly string connectionString;

        public TransactionDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public decimal SendMoney(int senderId, int receiverId, decimal amount)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    AccountDao account = new AccountDao(connectionString);
                    decimal balance = account.GetBalance(senderId);
                    if (amount <= balance)
                    {
                        conn.Open();

                        SqlCommand add = new SqlCommand("BEGIN TRANSACTION;" +
                                "UPDATE accounts SET balance = balance + @amount WHERE user_id = @receiverId;" +
                                "UPDATE accounts SET balance = balance - @amount WHERE user_id = @senderId;" +
                                "COMMIT;", conn);
                        add.Parameters.AddWithValue("@senderId", senderId);
                        add.Parameters.AddWithValue("@receiverId", receiverId);
                        add.Parameters.AddWithValue("@amount", amount);

                        SqlDataReader reader = add.ExecuteReader();

                        return account.GetBalance(senderId);
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            return 0;
        }
        public List<Transaction> ViewTransactions(int accountID)
        {
            return null;
        }
        public Transaction ViewTransactionDetails(int transactionID)
        {
            return null;
        }
    }
}
