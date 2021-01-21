using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentForumAspCore.Models
{
    public class JoinGroupModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool RequirePassword { get; set; }

        [RequiredIf("RequirePassword", ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
