using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class Topic
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        [Required]
        [ForeignKey("Author")]
        public string UserId { get; set; }
        public User Author { get; set; }

        public DateTime Date { get; set; }

        public List<TopicAnswer> Answers { get; set; }

        [Required]
        [ForeignKey("Group")]
        public int GroupId { get; set; }
        public Group Group { get; set; }
    }
}
