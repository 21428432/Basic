using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZaKhan.Model
{
  public  class FeeView
  {
        [Key]
        public int FeeId { get; set; }

        [Required]
        [Display(Name = "Price")]
        public double Price { get; set; }

        [Required]
        [Display(Name = "Vat")]
        public double Vat { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}
