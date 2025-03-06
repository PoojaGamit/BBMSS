using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBMSDATA1.Models
{
    public class BloodCamps
    {
        [Key]
        public int CampId { get; set; }

        [Required]
        [MaxLength(255)]
        public string? Name { get; set; }

        [Required]
        [MaxLength(255)]
        public string? Location { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [Required]
        [MaxLength(255)]
        public string? Organizer { get; set; }

        [Required]
        public DateTime VisibilityUntil { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

    
    }
}
