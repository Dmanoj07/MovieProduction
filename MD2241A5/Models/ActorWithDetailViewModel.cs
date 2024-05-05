using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MD2241A5.Models
{
    public class ActorWithDetailViewModel : ActorBaseViewModel
    {
        public IEnumerable<ShowBaseViewModel> Shows { get; set; }
    }
}