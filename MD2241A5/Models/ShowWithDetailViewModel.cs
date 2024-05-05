using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MD2241A5.Models
{
    public class ShowWithDetailViewModel:ShowBaseViewModel
    {
        public int Id { get; set; }
        public IEnumerable<ActorBaseViewModel> Actors { get; set; }
        public IEnumerable<EpisodeBaseViewModel> Episodes { get; set; }
    }
}