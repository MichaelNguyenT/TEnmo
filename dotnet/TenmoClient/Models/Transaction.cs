using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TenmoClient.Models
{
    public class Transaction
    {
        public int? Id{get; set;}

        [Required(ErrorMessage = "the field 'FromName' should not be blank.")]
        public int FromId { get; set; }

        [Required(ErrorMessage = "the field 'FromUserName' should not be blank.")]
        public string FromUserName { get; set; }

        [Required(ErrorMessage = "the field 'ToName' should not be blank.")]
        public int ToId { get; set; }

        [Required(ErrorMessage = "the field 'ToUserName' should not be blank.")]
        public string ToUserName { get; set; }

        [Required(ErrorMessage = "the field 'Type' should not be blank.")]
        public string Type { get; set; }

        public string Status { get; set; }

        [Required(ErrorMessage = "the field 'Amount' should not be blank.")]
        public decimal Amount { get; set; }
    }
}
