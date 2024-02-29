using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace Assignment3.Models
{
    public class Candy
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public double Weight { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public float? Cost { get; set; }

        [DataType(DataType.Upload)]
        [DisplayName("Product Image")]
        public byte[] ProductImage { get; set; }

        [DisplayName("Is It Healthy?")]
        public bool IsHealthy { get; set; }
    }
}
