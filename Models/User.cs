using System.ComponentModel.DataAnnotations;

namespace NewShoppingCart.Models
{
    public class User
    {
        public string Id { get; set; }
        [Required(ErrorMessage ="Please enter Username")]
        public string Username { get; set; }
        [EmailAddress(ErrorMessage = "Please enter a value email address")]
        [Required(ErrorMessage = "Please enter Email")]
        public string Email { get; set; }
        public string Contact { get;set; }
    }
}
