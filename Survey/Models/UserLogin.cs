using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Survey.Models
{
    public class UserLogin
    {
        [Display(Name = "Email Id")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email id required")]
        [DataType(DataType.EmailAddress)]
        public string UserEmailID { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password name required")]
        [DataType(DataType.Password)]

        public string Password { get; set; }
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

    }
    public class AdminLogin
    {
        [Display(Name = "Email Id")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email id required")]
        [DataType(DataType.EmailAddress)]
        public string EmailID { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password name required")]
        [DataType(DataType.Password)]

        public string Password { get; set; }
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

    }
}
