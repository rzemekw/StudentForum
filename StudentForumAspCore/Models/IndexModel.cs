using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentForumAspCore.Models
{
    public class IndexModel
    {
        public List<IndexTopicModel> UnseenTopics { get; set; }
        public List<IndexTopicModel> TopicsWithNewAnswers { get; set; }
    }
}
