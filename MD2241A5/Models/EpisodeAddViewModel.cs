using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MD2241A5.Models
{
    public class EpisodeAddViewModel
    {
        public string Name { get; set; }
        public int SeasonNumber { get; set; }
        public int EpisodeNumber { get; set; }
        public DateTime? AirDate { get; set; }
        public string ImageUrl { get; set; }
        public string ShowName { get; set; }

        public int ShowId { get; set; } 
        public string Clerk { get; set; }
    }
}