using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class TransactionDao
    {
        public ActionResult SendMoney(int userID, decimal Amount)
        {
            return null;
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
