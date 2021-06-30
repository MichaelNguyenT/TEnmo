using System.ComponentModel.DataAnnotations;

namespace TenmoClient.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "the field 'Username' should not be blank.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "the field 'Password' should not be blank.")]
        public string PasswordHash { get; set; }

        [Required(ErrorMessage = "the field 'Salt' should not be blank.")]
        public string Salt { get; set; }

        [Required(ErrorMessage = "the field 'Email' should not be blank.")]
        public string Email { get; set; }
    }
}
