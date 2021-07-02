using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TenmoServer.DAO;
using TenmoServer.Models;
using Microsoft.AspNetCore.Authorization;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionDao _transactionDao;
        private readonly IUserDao _userDao;

        public TransactionController(ITransactionDao transactionDao, IUserDao userDao)
        {
            _transactionDao = transactionDao;
            _userDao = userDao;
        }

        [HttpPut("/account/{receiverId}/{amount}")]
        public decimal SendMoney(int receiverId, decimal amount)
        {
            User user = _userDao.GetUser(User.Identity.Name);
            return _transactionDao.SendMoney(user.UserId, receiverId, amount);
        }

        [HttpPut("/request/{requesteeId}/{amount}")]
        public string RequestTransaction(int requesteeId, decimal amount)
        {
            User user = _userDao.GetUser(User.Identity.Name);
            Transaction transaction = _transactionDao.RequestTransactionCreation(user.UserId, requesteeId, amount);
            if (transaction != null)
            {
                return $"You have made a request from {requesteeId} for ${amount}";
            }
            else
            {
                return "Transaction Failed, please try again";
            }
        }

        [HttpPut("/completerequest/{transactionId}/{userChoice}")]
        public string CompleteTransaction(int transactionId, int userChoice)
        {
            return _transactionDao.TransactionStatusUpdate(transactionId, userChoice);
            
        }

        [HttpGet("/user")]
        public List<User> GetUsers()
        {
            List<User> users = new List<User>(_userDao.GetUsers());
            return users;
        }

        [HttpGet("/transaction")]
        public List<Transaction> GetTransactions()
        {
            User user = _userDao.GetUser(User.Identity.Name);
            int accountId =_userDao.GetAccount(user.UserId);
            List<Transaction> transactions = _transactionDao.ViewTransactions(accountId);
            return transactions;
        }

        [HttpGet("/pendingtransaction")]
        public List<Transaction> GetPendingTransactions()
        {
            User user = _userDao.GetUser(User.Identity.Name);
            int accountId = _userDao.GetAccount(user.UserId);
            List<Transaction> transactions = _transactionDao.ViewPendingTransactions(accountId);
            return transactions;
        }
    }
}
