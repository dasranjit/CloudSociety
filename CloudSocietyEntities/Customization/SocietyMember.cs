using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DataAnnotationsExtensions;

namespace CloudSocietyEntities
{
    //Class To validate the EmailID and Password in SocietyMember Create add by Ranjit
    public partial class SocietyMember
    {
        //[DataType(DataType.EmailAddress)]
        //[Display(Name = "Email address")]
        //[Email]
        //public string Email { get; set; }

        [RegularExpression("[\\S]{6,}", ErrorMessage = "Password must be at least 6 characters.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        //string PasswordQuestion,string PasswordAnswer, bool OfficeBearer
        [DataType(DataType.Text)]
        [Display(Name = "Password Question")]
        public string PasswordQuestion { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Password Answer")]
        public string PasswordAnswer { get; set; }

        [Display(Name = "Office Bearer")]
        public bool OfficeBearer { get; set; }


    }
}
