using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class UserTopicDate
    {
        [ForeignKey("Topic")]
        public int TopicId { get; set; }
        public Topic Topic { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }

        public DateTime LastVisited { get; set; }
    }
}
