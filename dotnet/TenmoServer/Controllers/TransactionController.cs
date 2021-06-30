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

        [HttpGet("/user")]
        public List<User> GetUsers()
        {
            List<User> users = new List<User>(_userDao.GetUsers());
            return users;
        }
    }
}
