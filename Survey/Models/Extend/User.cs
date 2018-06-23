using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Survey.Models.Extend
{
    [MetadataType(typeof(UserMetadata))]
    public partial class User
    {
        public string ConfirmPassword { get; set; }
    }
    public class UserMetadata
    {
        [Display(Name = "Email Id")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email id required")]
        [DataType(DataType.EmailAddress)]
        public string UserEmailID { get; set; }

        [Display(Name = "UserName")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "User name required")]
        public string Username { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Password name required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Minimum 6 character required ")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password do not match")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Designation")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Designation is required")]
        public string Designation { get; set; }

        public bool IsEmailVarify { get; set; }
        public Nullable<int> SurveyCreated { get; set; }

        public System.Guid ActivationCode { get; set; }
    }
}