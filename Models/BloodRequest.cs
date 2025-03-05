using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBMSDATA1.Models
{
    public class BloodRequest
    {
        [Key]
        public int RequestId { get; set; }
        public int PatientId { get; set; }
        public int BloodGroupId { get; set; }
        public int RequiredQuantity { get; set; }
        public DateTime RequiredDate { get; set; }
        public int BloodBankId { get; set; }
        public string ?Status { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Now;

      
    }
}
