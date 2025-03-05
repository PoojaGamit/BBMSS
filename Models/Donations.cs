using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBMSDATA1.Models
{
    public class Donations
    {
        [Key]
        public int DonationId { get; set; }
        public int DonorId { get; set; }
        public int BloodBankId { get; set; }
        public DateTime DonationDate { get; set; } = DateTime.Now;
        public int DonationTypeId { get; set; }
        public int ComponentTypeId { get; set; }
        public int BagTypeId { get; set; }
        public int Quantity { get; set; }
        public string ?Status { get; set; }

       
    }
}
