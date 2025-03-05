using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBMSDATA1.Models
{
  public class Notification
    {
        [Key]
        public int NotificationId { get; set; }
        public int BloodBankId { get; set; }
        public string ?Title { get; set; }
        public string ?Message { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
