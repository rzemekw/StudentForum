using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class Group
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        [MaxLength(512)]
        public string PasswordHash { get; set; }

        [MaxLength(512)]
        public string PasswordSalt { get; set; }

        [Required]
        [ForeignKey("Admin")]
        public string AdminId { get; set; }
        public User Admin { get; set; }

        [ForeignKey("University")]
        public int? UniversityId { get; set; }
        public University University { get; set; }

        public List<Topic> Topics { get; set; }

        public List<UserGroup> UserGroups { get; set; }
    }
}
