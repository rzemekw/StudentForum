using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class TopicAnswer
    {
        public int Id { get; set; }

        [MaxLength(2048)]
        public string Content { get; set; }

        public DateTime Date { get; set; }

        public List<Attachement> Attachements { get; set; }

        [Required]
        [ForeignKey("Author")]
        public string UserId { get; set; }
        public User Author { get; set; }

        [Required]
        [ForeignKey("Topic")]
        public int TopicId { get; set; }
        public Topic Topic { get; set; }
    }
}
