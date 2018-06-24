using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Survey.Models
{
    [MetadataType(typeof(AdminMetadata))]
    public partial class Admin
    {
        public string ConfirmPassword { get; set; }
    }
    public class AdminMetadata
    {
        [Display(Name = "Email Id")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email id required")]
        [DataType(DataType.EmailAddress)]
        public string EmailID { get; set; }

        [Display(Name = "AdminName")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Admin name required")]
        public string AdminName { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Password name required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Minimum 6 character required ")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password do not match")]
        public string ConfirmPassword { get; set; }


        public System.Guid ActivationCode { get; set; }
    }
}