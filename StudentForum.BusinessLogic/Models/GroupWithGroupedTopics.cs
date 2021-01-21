using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentForum.BusinessLogic.Models
{
    public class GroupWithGroupedTopics
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<Topic> UnseenTopics { get; set; }
        public List<Topic> TopicsWithNewAnswers { get; set; }
        public List<Topic> OldTopics { get; set; }

    }
}
