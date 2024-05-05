﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MD2241A5.Models
{
    public class ShowAddViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string Name { get; set; }
        public string Genre { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReleaseDate { get; set; }
        [Required]
        [StringLength(250)]
        public string ImageUrl { get; set; }
        public IEnumerable<int> SelectedActorIds { get; set; }
    }
}