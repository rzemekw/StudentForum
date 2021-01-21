using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentForumAspCore.Models
{
    public class CreateTopicAnswerModel
    {
        [Required]
        public int TopicId { get; set; }

        [Required]
        public string Content { get; set; }

        public List<AttachementModel> Attachements { get; set; }
    }
}