using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MvcMovie.Models
{
    public class Movie
    {
        public int ID { get; set; }

        [StringLength(60, MinimumLength = 3)]
        [Required (ErrorMessage = "Please enter a title!")]
        public string Title { get; set; }

        [Display(Name = "Release Date")]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}")]
        [DataType(DataType.Date)]
        [Required (ErrorMessage = "Please enter a release date!")]
        public DateTime? ReleaseDate { get; set; }

        [Required (ErrorMessage = "Please enter a genre!")]
        public string Genre { get; set; }

        [Range(1, 100)]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        [Required (ErrorMessage = "Please enter a price!")]
        public decimal Price { get; set; }

        [Required (ErrorMessage = "Please enter a rating!")]
        public string Rating { get; set; }

        public string Poster { get; set; }
    }
}
