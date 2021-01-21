using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentForumAspCore.Models
{
    public class MenuUniversityGroupsModel
    {
        public List<MenuGroupModel> Groups {get;set;}
        public string UniversityName { get; set; }
        public int? UniversityId { get; set; }
    }
}
