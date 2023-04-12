using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        [DisplayName("Phone Number")]
        public string? PhoneNumber { get; set; }

    }
}
