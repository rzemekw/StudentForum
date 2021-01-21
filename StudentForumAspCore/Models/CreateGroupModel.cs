using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentForumAspCore.Models
{
    public class CreateGroupModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        public string Name { get; set; }

        [Display(Name = "Bind to university")]
        public bool RequireUniversity { get; set; }

        [Display(Name = "University")]
        public List<SelectListItem> Universities { get; set; }

        [RequiredIf("RequireUniversity", ErrorMessage = "You need to select a university if you decided to bind this group to some")]
        public int? UniversityId { get; set; }


        [Display(Name = "Require password to join")]
        public bool RequirePassword { get; set; }


        [DataType(DataType.Password)]
        [RequiredIf("RequirePassword", ErrorMessage ="You need to provide a password if you require it")]
        [StringLength(100, ErrorMessage = "Password cannot be more than {1} characters long")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
