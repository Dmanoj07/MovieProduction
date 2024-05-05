using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MD2241A5.Data
{
    public class Show
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Genre { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(250)]
        public string ImageUrl { get; set; }

        [Required]
        [StringLength(250)]
        public string Coordinator { get; set; }

        // Navigation properties
        public ICollection<Actor> Actors { get; set; } = new HashSet<Actor>();
        public ICollection<Episode> Episodes { get; set; } = new HashSet<Episode>();
    }
}