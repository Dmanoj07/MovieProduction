using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MD2241A5.Models
{
    

    public class EpisodeAddFormViewModel
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [Required]
        public int SeasonNumber { get; set; }

        [Required]
        public int EpisodeNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? AirDate { get; set; }

        [Required]
        [StringLength(250)]
        public string ImageUrl { get; set; }

        [Required]
        public string Genre { get; set; }
        public SelectList GenreList { get; set; }

        public int ShowId { get; set; }
        public string ShowName { get; set; }  // Display the show's name

        public string Clerk { get; set; } // Automatically set based on logged-in user
    }
}