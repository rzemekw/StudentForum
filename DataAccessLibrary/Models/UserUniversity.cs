using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class UserUniversity
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public int UniversityId { get; set; }
        public University University { get; set; }
    }
}
