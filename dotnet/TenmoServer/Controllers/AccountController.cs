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
    public class AccountController : ControllerBase
    {
        private readonly IAccountDao _dao;
        private readonly IUserDao _userDao;

        public AccountController(IAccountDao accountDao, IUserDao userDao)
        {
            _dao = accountDao;
            _userDao = userDao;
        }

        [Authorize]
        [HttpGet("/account")]
        public decimal GetBalance()
        {
            User user = _userDao.GetUser(User.Identity.Name);
            decimal returnAccount = _dao.GetBalance(user.UserId);
            return returnAccount;
            //User.findfirst("sub").value
        }

    }
}
