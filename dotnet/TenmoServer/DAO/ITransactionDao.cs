using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace TenmoServer.DAO
{
    interface ITransactionDao
    {
        ActionResult SendMoney(int accountID, decimal amount);
        List<Transaction> ViewTransactions(int accountID);
        Transaction ViewTransactionDetails(int transactionID);
    }
}