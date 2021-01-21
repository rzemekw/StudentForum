using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentForum.BusinessLogic.Models
{
    public class GroupWithNotifications
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool NewTopicAvailable { get; set; }
        public bool NewAnswerAvailable { get; set; }
    }
}
