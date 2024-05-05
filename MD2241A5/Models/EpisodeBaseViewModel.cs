using MD2241A5.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MD2241A5.Models
{
   
    public class EpisodeBaseViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string Name { get; set; }
        [Required]
        public int SeasonNumber { get; set; }
        [Required]
        public int EpisodeNumber { get; set; }
        [Required]
        [StringLength(50)]
        public string Genre { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime AirDate { get; set; }
        [Required]
        [StringLength(250)]
        public string ImageUrl { get; set; }
        [Required]
        [StringLength(250)]
        public string Clerk { get; set; }
        public ShowBaseViewModel Show { get; set; }
    }
}