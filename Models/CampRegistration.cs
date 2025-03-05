using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBMSDATA1.Models
{
   public class CampRegistration
    {
        [Key]
        public int RegistrationId { get; set; }

        [Required]
        public int CampId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Registered";

        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    }
}
