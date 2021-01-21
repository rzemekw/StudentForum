using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentForumAspCore.Models
{
    public class DetailsGroupModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<DetailsTopicModel> UnseenTopics { get; set; }
        public List<DetailsTopicModel> TopicsWithNewAnswers { get; set; }
        public List<DetailsTopicModel> OldTopics { get; set; }
    }
}
