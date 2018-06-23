using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Survey.Models
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "New Password required", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "New password and confirm password not match")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string ResetCode { get; set; }   //get from the reset password link

    }
}