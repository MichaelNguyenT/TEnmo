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
        public int FromId { get; private set; }

        [Required(ErrorMessage = "the field 'ToName' should not be blank.")]
        public int ToId { get; private set; }

        [Required(ErrorMessage = "the field 'Type' should not be blank.")]
        public string Type { get; private set; }

        public string Status { get; set; }

        [Required(ErrorMessage = "the field 'Amount' should not be blank.")]
        public decimal Amount { get; private set; }
        public Transaction CreateTransaction(int senderUsername, int receiverUsername, decimal amount, string transactionType)
        {
            Transaction transaction = new Transaction();
            transaction.FromId = senderUsername;
            transaction.ToId = receiverUsername;
            transaction.Amount = amount;
            transaction.Type = transactionType;
            if (transactionType == "Send")
            {
                transaction.Status = "Approved";
            }
            else
            {
                transaction.Status = "Pending";
            }

            return transaction;
        }
    }
}
