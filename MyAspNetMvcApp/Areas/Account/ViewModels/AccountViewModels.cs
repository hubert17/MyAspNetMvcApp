using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MyAspNetMvcApp.Models;
using MyAspNetMvcApp.Areas.Account.Models;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MyAspNetMvcApp.Areas.Account.ViewModels
{
    public partial class RegisterViewModel
    {
        [Required(ErrorMessage = "You can't leave this empty.")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [System.Web.Mvc.Remote("CheckExistingEmail", "Account", ErrorMessage = "Email already exists")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "You can't leave this empty.")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 10)]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Mobile phone")]
        [System.Web.Mvc.Remote("CheckExistingPhoneNumber", "Account", ErrorMessage = "Phone number already exists")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "You can't leave this empty.")]
        [DataType(DataType.Date)]
        [Display(Name = "Birthday")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? BirthDate { get; set; }

        public string Gender { get; set; }

        public string CountryCode { get; set; }

        public string RegistrationType { get; set; }

        public byte[] Picture { get; set; }

        [Required(ErrorMessage = "You can't leave this empty.")]
        [Display(Name = "Lastname")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "You can't leave this empty.")]
        [Display(Name = "Firstname")]
        public string FirstName { get; set; }

        public static string AddRole(string UserName, string RoleName)
        {
            if(RoleName != System.Configuration.ConfigurationManager.AppSettings["AdminRolename"]) // Restrict adding of ADMIN role
            {
                try
                {
                    using (var db = new ApplicationDbContext())
                    {
                        var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
                        if (!db.Roles.Any(r => r.Name == RoleName))
                        {
                            roleManager.Create(new IdentityRole(RoleName));
                        }

                        ApplicationUser user = db.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                        var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                        userManager.AddToRole(user.Id, RoleName);
                    }
                    return RoleName;
                }
                catch{}
            }
            return string.Empty;
        }

    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter a valid email")]
        [Display(Name = "Email")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [System.Web.Mvc.HiddenInput(DisplayValue = false)]
        public string Email { get; set; }

        [System.Web.Mvc.HiddenInput(DisplayValue = false)]
        public string Code { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class VerifyPhoneNumberViewModel
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }
}
