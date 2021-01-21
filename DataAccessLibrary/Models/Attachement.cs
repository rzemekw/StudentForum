using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class Attachement
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Path { get; set; }

        [Required]
        [MaxLength(200)]
        public string FileName { get; set; }

        [Required]
        [ForeignKey("TopicAnswer")]
        public int TopicAnswerId { get; set; }
        public TopicAnswer TopicAnswer {get;set;}
    }
}
