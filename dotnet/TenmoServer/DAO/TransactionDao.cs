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
            //decimal balance = 0.0M;
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

                        Transaction transaction = new Transaction();
                        UserSqlDao userSqlDao = new UserSqlDao(connectionString);
                        AddTransaction(transaction.CreateTransaction(userSqlDao.GetAccount(senderId), userSqlDao.GetAccount(receiverId), amount, "Send"));

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
            List<Transaction> transactionList = new List<Transaction>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT transfer_id, transfer_type_id, transfer_status_id, account_from, account_to, amount " +
                                        "FROM transfers " +
                                        "WHERE account_from = @accountId", conn);
                    cmd.Parameters.AddWithValue("@accountId", accountID);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        transactionList.Add(reader);
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            return null;
        }
        public Transaction ViewTransactionDetails(int transactionID)
        {
            return null;
        }

        public void AddTransaction(Transaction transaction)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    //Default values are Approved and Request until stated otherwise
                    int transferStatusId = 2, transferTypeId = 1;

                    if (transaction.Type == "Send")
                    {
                        transferTypeId = 2;
                    }
                    if (transaction.Status == "Pending")
                    {
                        transferStatusId = 1;
                    }
                    else if (transaction.Status == "Rejected")
                    {
                        transferStatusId = 3;
                    }

                    SqlCommand cmd = new SqlCommand("INSERT INTO transfers(account_from, account_to, amount, transfer_type_id, transfer_status_id)" +
                                            " VALUES(@accountFrom, @accountTo, @amount, @transferType, @transferStatus)", conn);
                    cmd.Parameters.AddWithValue("@accountFrom", transaction.FromId);
                    cmd.Parameters.AddWithValue("@accountTo", transaction.ToId);
                    cmd.Parameters.AddWithValue("@amount", transaction.Amount);
                    cmd.Parameters.AddWithValue("@transferType", transferTypeId);
                    cmd.Parameters.AddWithValue("@transferStatus", transferStatusId);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {

                Console.WriteLine(e.Message);
            }
        }

        private Transaction GetTransactionFromReader(SqlDataReader reader)
        {
            int typeAsInt = Convert.ToInt32(reader["transfer_type_id"]);
            int statusAsInt = Convert.ToInt32(reader["transfer_status_id"]);
            string transactionType = "", transactionStatus = "";
            if (typeAsInt == 1) 
            {
                transactionType = "Request";
            }
            else
            {
                transactionType = "Send";
            }

            if (statusAsInt == 1)
            {
                transactionStatus = "Pending";
            }
            else if (statusAsInt == 2)
            {
                transactionStatus = "Approved";
            }
            else
            {
                transactionStatus = "Rejected";
            }

            Transaction transaction = new Transaction()
            {
                Id = Convert.ToInt32(reader["transfer_id"]),
                Type = transactionType,
                Status = transactionStatus,
                FromId = Convert.ToInt32(reader["account_from"]),
                ToId = Convert.ToInt32(reader["account_to"]),
                Amount = Convert.ToInt32(reader["amount"]),
            };
            return transaction;
        }
    }
}
