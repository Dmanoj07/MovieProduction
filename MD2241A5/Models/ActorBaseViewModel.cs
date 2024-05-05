using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MD2241A5.Models
{
    public class ActorBaseViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string Name { get; set; }
        [StringLength(150)]
        public string AlternateName { get; set; }
        [Required]
        [StringLength(250)]
        public string ImageUrl { get; set; }
        public double? Height { get; set; }
        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? BirthDate { get; set; }
        [Required]
        [StringLength(250)]
        public string Executive { get; set; }
        public bool Selected { get; set; }
        public string Text { get; set; }
    }
}