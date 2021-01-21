using System;
using System.Collections.Generic;

namespace StudentForumAspCore.Models
{
    public class TopicAnswerModel
    {
        public string Content { get; set; }

        public DateTime Date { get; set; }

        public List<AttachementModel> Attachements { get; set; }

        public UserModel Author { get; set; }
    }
}