using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MD2241A5.Models
{
    public class EpisodeWithDetailViewModel: EpisodeBaseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SeasonNumber { get; set; }
        public int EpisodeNumber { get; set; }
        public string Genre { get; set; }
        public DateTime AirDate { get; set; }
        public string ImageUrl { get; set; }
        public ShowBaseViewModel Show { get; set; }
    }
}