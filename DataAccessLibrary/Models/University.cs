using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class University
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        public List<Group> Groups { get; set; }

        public List<UserUniversity> UserUniversities { get; set; }

        [Required]
        [ForeignKey("Country")]
        public string CountryId { get; set; }
        public Country Country { get; set; }

        [MaxLength(100)]
        public string Website { get; set; }
    }
}
