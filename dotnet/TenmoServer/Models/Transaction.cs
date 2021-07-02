using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TenmoServer.Models
{
    public class Transaction
    {
        public int? Id{get; set;}

        [Required(ErrorMessage = "the field 'FromAccountId' should not be blank.")]
        public int FromAccountId { get; set; }

        public string FromUserName { get; set; }

        public int FromUserId { get; set; }

        [Required(ErrorMessage = "the field 'ToAccountId' should not be blank.")]
        public int ToAccountId { get; set; }

        public string ToUserName { get; set; }

        public int ToUserId { get; set; }

        [Required(ErrorMessage = "the field 'Type' should not be blank.")]
        public string Type { get; set; }

        public string Status { get; set; }

        [Required(ErrorMessage = "the field 'Amount' should not be blank.")]
        public decimal Amount { get; set; }
        public Transaction CreateTransaction(int fromAccountId, string fromUsername, int fromUserId, int toAccountId, string toUsername, int toUserId, decimal amount, string transactionType)
        {
            Transaction transaction = new Transaction();
            transaction.FromAccountId = fromAccountId;
            transaction.FromUserName = fromUsername;
            transaction.FromUserId = fromUserId;
            transaction.ToAccountId = toAccountId;
            transaction.ToUserName = toUsername;
            transaction.ToUserId = toUserId;
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
        public Transaction CreateTransaction(int fromAccountId, int toAccountId, decimal amount, string transactionType)
        {
            Transaction transaction = new Transaction();
            transaction.FromAccountId = fromAccountId;
            transaction.ToAccountId = toAccountId;
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
