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

        public decimal SendMoney(int senderAccountId, int receiverAccountId, decimal amount)
        {
            //decimal balance = 0.0M;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    AccountDao account = new AccountDao(connectionString);
                    decimal balance = account.GetBalance(senderAccountId);
                    if (amount <= balance)
                    {
                        conn.Open();

                        SqlCommand add = new SqlCommand("BEGIN TRANSACTION;" +
                                "UPDATE accounts SET balance = balance + @amount WHERE user_id = @receiverId;" +
                                "UPDATE accounts SET balance = balance - @amount WHERE user_id = @senderId;" +
                                "COMMIT;", conn);
                        add.Parameters.AddWithValue("@senderId", senderAccountId);
                        add.Parameters.AddWithValue("@receiverId", receiverAccountId);
                        add.Parameters.AddWithValue("@amount", amount);

                        SqlDataReader reader = add.ExecuteReader();

                        Transaction transaction = new Transaction();
                        UserSqlDao userSqlDao = new UserSqlDao(connectionString);
                        AddTransaction(transaction.CreateTransaction(userSqlDao.GetAccount(senderAccountId), userSqlDao.GetAccount(receiverAccountId), amount, "Send"));

                        return account.GetBalance(senderAccountId);
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

                    SqlCommand cmd = new SqlCommand("  SELECT t.transfer_id, t.transfer_type_id, t.transfer_status_id, t.account_from, t.account_to, t.amount, a1.user_id AS from_user_id, a2.user_id AS to_user_id, u1.username AS from_username, u2.username AS to_username "+
                                    "FROM transfers t " +
                                    "JOIN accounts a1 ON a1.account_id = t.account_from " +
                                    "JOIN accounts a2 ON a2.account_id = t.account_to " +
                                    "JOIN users u1 ON u1.user_id = a1.user_id " +
                                    "JOIN users u2 ON u2.user_id = a2.user_id " +
                                    "WHERE t.account_from = @fromid OR t.account_to = @toid", conn);
                    cmd.Parameters.AddWithValue("@fromId", accountID);
                    cmd.Parameters.AddWithValue("@toid", accountID);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        transactionList.Add(GetTransactionFromReader(reader));
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            return transactionList;
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
                    cmd.Parameters.AddWithValue("@accountFrom", transaction.FromAccountId);
                    cmd.Parameters.AddWithValue("@accountTo", transaction.ToAccountId);
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
            string transactionType, transactionStatus;
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
                FromAccountId = Convert.ToInt32(reader["account_from"]),
                FromUserName = Convert.ToString(reader["from_username"]),
                FromUserId = Convert.ToInt32(reader["from_user_id"]),
                ToAccountId = Convert.ToInt32(reader["account_to"]),
                ToUserName = Convert.ToString(reader["to_username"]),
                ToUserId = Convert.ToInt32(reader["to_user_id"]),
                Amount = Convert.ToInt32(reader["amount"]),
            };
            return transaction;
        }
    }
}
