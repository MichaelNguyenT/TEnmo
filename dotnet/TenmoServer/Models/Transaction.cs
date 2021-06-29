using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TenmoServer.Models
{
    public class Transaction
    {
        public int? Id{get; private set;}

        [Required(ErrorMessage = "the field 'FromName' should not be blank.")]
        public string FromName { get; private set; }

        [Required(ErrorMessage = "the field 'ToName' should not be blank.")]
        public string ToName { get; private set; }

        [Required(ErrorMessage = "the field 'Type' should not be blank.")]
        public string Type { get; private set; }

        public string Status { get; private set; }

        [Required(ErrorMessage = "the field 'Amount' should not be blank.")]
        public decimal Amount { get; private set; }
    }
}
