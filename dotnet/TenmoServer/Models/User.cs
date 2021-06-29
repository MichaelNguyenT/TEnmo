using System.ComponentModel.DataAnnotations;

namespace TenmoServer.Models
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

    /// <summary>
    /// Model to return upon successful login
    /// </summary>
    public class ReturnUser
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        //public string Role { get; set; }
        public string Token { get; set; }
    }

    /// <summary>
    /// Model to accept login parameters
    /// </summary>
    public class LoginUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
