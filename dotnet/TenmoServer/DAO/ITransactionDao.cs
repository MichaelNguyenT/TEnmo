using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace TenmoServer.DAO
{
    public interface ITransactionDao
    {
        decimal SendMoney(int senderId,int receiverId, decimal amount);
        List<Transaction> ViewTransactions(int accountID);
        Transaction RequestTransactionCreation(int requesterId, int requesteeId, decimal amount);
        public string TransactionStatusUpdate(int transactionId, int userChoice);
        public List<Transaction> ViewPendingTransactions(int accountId);
    }
}