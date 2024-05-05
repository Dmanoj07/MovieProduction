using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MD2241A5.Models
{
   

    public class ShowAddFormViewModel
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; }
        public SelectList Genres { get; set; } // Updated to use SelectList
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReleaseDate { get; set; }
        [Required]
        [StringLength(250)]
        public string ImageUrl { get; set; }
        public string KnownActorName { get; set; }

        public string Genre { get; set; }

        public IEnumerable<int> SelectedActorIds { get; set; }
        public MultiSelectList Actors { get; set; } // For selecting multiple actors

    }

}