using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentForumAspCore.Models
{
    public class CreateTopicModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int GroupId { get; set; }
    }
}
