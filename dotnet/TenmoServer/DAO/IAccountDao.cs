using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;
using Microsoft.AspNetCore.Mvc;


namespace TenmoServer.DAO
{
    public interface IAccountDao
    {
        string GetBalance();
    }
}