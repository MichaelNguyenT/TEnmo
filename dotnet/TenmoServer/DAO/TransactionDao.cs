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

        public string SendMoney(int senderAccountId, int receiverAccountId, decimal amount)
        {
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
                        AddTransaction(transaction.CreateTransaction(userSqlDao.GetAccountId(senderAccountId), userSqlDao.GetAccountId(receiverAccountId), amount, "Send"));

                        return $"You sent ${amount} to user {userSqlDao.GetAccountId(receiverAccountId)}. Your remaining balance is ${account.GetBalance(senderAccountId)}";
                    }
                    else
                    {
                        return "Please enter a valid amount to send";
                    }
                }
            }
            catch (SqlException e)
            {
                return e.Message + " Transaction Failed";
            }
        }

        public Transaction TransactionAtId(int transactionId)
        {
            Transaction transaction = new Transaction();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("  SELECT t.transfer_id, t.transfer_type_id, t.transfer_status_id, t.account_from, t.account_to, t.amount, a1.user_id AS from_user_id, a2.user_id AS to_user_id, u1.username AS from_username, u2.username AS to_username " +
                                    "FROM transfers t " +
                                    "JOIN accounts a1 ON a1.account_id = t.account_from " +
                                    "JOIN accounts a2 ON a2.account_id = t.account_to " +
                                    "JOIN users u1 ON u1.user_id = a1.user_id " +
                                    "JOIN users u2 ON u2.user_id = a2.user_id " +
                                    "WHERE t.transfer_id = @transferId", conn);
                    cmd.Parameters.AddWithValue("@transferId", transactionId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        transaction = GetTransactionFromReader(reader);
                    }
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message); ;
            }
            return transaction;
        }
    
        public List<Transaction> ViewPendingTransactions(int accountId)
        {
            List<Transaction> transactionList = new List<Transaction>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("  SELECT t.transfer_id, t.transfer_type_id, t.transfer_status_id, t.account_from, t.account_to, t.amount, a1.user_id AS from_user_id, a2.user_id AS to_user_id, u1.username AS from_username, u2.username AS to_username " +
                                    "FROM transfers t " +
                                    "JOIN accounts a1 ON a1.account_id = t.account_from " +
                                    "JOIN accounts a2 ON a2.account_id = t.account_to " +
                                    "JOIN users u1 ON u1.user_id = a1.user_id " +
                                    "JOIN users u2 ON u2.user_id = a2.user_id " +
                                    "WHERE t.transfer_status_id = 1 AND (t.account_from = @fromid OR t.account_to = @toid)", conn);
                    cmd.Parameters.AddWithValue("@fromId", accountId);
                    cmd.Parameters.AddWithValue("@toid", accountId);
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
        public List<Transaction> ViewTransactions(int accountId)
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
                    cmd.Parameters.AddWithValue("@fromId", accountId);
                    cmd.Parameters.AddWithValue("@toid", accountId);
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

        public Transaction RequestTransactionCreation(int requesterUserId, int requesteeUserId, decimal amount)
        {
            Transaction transaction = new Transaction();
            UserSqlDao userSqlDao = new UserSqlDao(connectionString);
            int requesterAccountId = userSqlDao.GetAccountId(requesterUserId);
            int requesteeAccountId = userSqlDao.GetAccountId(requesteeUserId);
            transaction = transaction.CreateTransaction(requesteeAccountId, requesterAccountId, amount, "Request");
            AddTransaction(transaction);
            return transaction;
        }

        public void UpdateTransactionSql(Transaction transaction, int transferStatusId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE transfers SET transfer_status_id = @transferStatus " +
                                          "WHERE transfer_id = @transferId", conn);
                    cmd.Parameters.AddWithValue("@transferStatus", transferStatusId);
                    cmd.Parameters.AddWithValue("@transferId", transaction.Id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
        }

        public void CompleteRequest(Transaction transaction) 
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE accounts SET balance = balance - @amount "+
                                        "WHERE account_id = @requesteeAccountId " +
                                        "UPDATE accounts SET balance = balance + @amount " +
                                        "WHERE account_id = @requesterAccountId", conn);
                    cmd.Parameters.AddWithValue("@requesteeAccountId", transaction.FromAccountId);
                    cmd.Parameters.AddWithValue("@requesterAccountId", transaction.ToAccountId);
                    cmd.Parameters.AddWithValue("amount", transaction.Amount);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
        }

        public string TransactionStatusUpdate(int transactionId, int userChoice)
        {
            Transaction transaction = TransactionAtId(transactionId);
            if (userChoice == 1)
            {
                transaction.Status = "Approved";
                UpdateTransactionSql(transaction, 2);
                CompleteRequest(transaction);
                return "Request Completed";
            }
            else if (userChoice == 2)
            {
                transaction.Status = "Rejected";
                UpdateTransactionSql(transaction, 3);
                return "Request Rejected";
            }
            else
            {
                return "No changes made.";
            }
        }
    }
}
