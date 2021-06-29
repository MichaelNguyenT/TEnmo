using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TenmoServer.Models
{
    public class Account : User
    {
        public int? AccountID { get; private set; }

        [Required(ErrorMessage = "the field 'Balance' should not be blank.")]
        public decimal Balance { get; private set; } = 1000.00M;
    }
}
