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
        string SendMoney(int senderId,int receiverId, decimal amount);
        List<Transaction> ViewTransactions(int accountID);
        Transaction RequestTransactionCreation(int requesterId, int requesteeId, decimal amount);
        string TransactionStatusUpdate(int transactionId, int userChoice);
        List<Transaction> ViewPendingTransactions(int accountId);
    }
}