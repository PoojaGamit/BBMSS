using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBMSDATA1.Models
{
    public class ComponentStock
    {
        [Key]
        public int CId { get; set; }
        public int BloodBankId { get; set; }
        public int BloodGroupId { get; set; }
        public int ComponentTypeId { get; set; }
        public int Quantity { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public DateTime ExpiryDate { get; set; }

    }
}
