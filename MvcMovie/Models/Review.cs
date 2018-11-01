using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MvcMovie.Models
{
    public class Review
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Please enter a name!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter a comment!")]
        public string Comment { get; set; }
        public int MovieID { get; set; }
        public Movie Movie { get; set; }
    }
}
