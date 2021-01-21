using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentForumAspCore.Models
{
    public class MenuGroupModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public bool NewTopicAvailable { get; set; }
        public bool NewAnswerAvailable { get; set; }
    }
}
