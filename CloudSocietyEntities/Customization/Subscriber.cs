using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DataAnnotationsExtensions;

namespace CloudSocietyEntities
{
    public partial class Subscriber
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        [Email]
        public string Email { get; set; }

        [Required]
        //[ValidatePasswordLength]
        [RegularExpression("[\\S]{6,}", ErrorMessage = "Password must be at least 6 characters.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Password Question")]
        public string PasswordQuestion { get; set; }

        [Required]
        [Display(Name = "Password Answer")]
        public string PasswordAnswer { get; set; }
    }
}
