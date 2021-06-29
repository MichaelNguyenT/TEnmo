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

    public class AccountController : ControllerBase
    {
        private readonly IAccountDao _dao;

        public AccountController(IAccountDao accountDao)
        {

        }

        [HttpGet]
        public string GetBalance()
        {
            string returnAccount = _dao.GetBalance();
            return returnAccount;
        }

    }
}
