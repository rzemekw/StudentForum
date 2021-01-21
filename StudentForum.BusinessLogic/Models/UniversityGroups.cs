using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentForum.BusinessLogic.Models
{
    public class UniversityGroups
    {
        public List<GroupWithNotifications> Groups { get; set; }
        public string UniversityName { get; set; }
        public int? UniversityId { get; set; }
    }
}
