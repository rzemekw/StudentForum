using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentForumAspCore.Models
{
    public class JoinUniversityModel
    {
        [Display(Name = "Country")]
        public List<SelectListItem> Countries { get; set; }

        [Display(Name = "University")]
        public List<SelectListItem> Universities { get; set; }

        [Required(ErrorMessage = "You need to choose a university")]
        public int? UniversityId { get; set; }
    }
}
