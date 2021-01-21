using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class User
    {
        [Key]
        [MaxLength(450)]
        public string Id { get; set;}

        [Required]
        [MaxLength(128)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(128)]
        public string LastName { get; set; }

        public List<UserUniversity> UserUniversities { get; set; }
        
        public List<UserGroup> UserGroups { get; set; }

        public List<Group> AdministratedGroups { get; set; }
    }
}
