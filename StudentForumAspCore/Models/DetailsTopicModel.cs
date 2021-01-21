using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentForumAspCore.Models
{
    public class DetailsTopicModel
    {
        public int Id { get; set; }

        public GroupModel Group { get; set; }

        public string Name { get; set; }

        public UserModel Author { get; set; }

        public DateTime Date { get; set; }

        public DateTime LastVisited { get; set; }
    }
}
