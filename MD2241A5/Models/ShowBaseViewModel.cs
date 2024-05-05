using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MD2241A5.Models
{
    public class ShowBaseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Genre { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReleaseDate { get; set; }
        public string ImageUrl { get; set; }
        public string Coordinator { get; set; }
        public ICollection<EpisodeBaseViewModel> Episodes { get; set; }
    }
}