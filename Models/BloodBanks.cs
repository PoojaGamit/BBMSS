using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBMSDATA1.Models
{
   public class BloodBanks
    {
        [Key]
        public int BloodBankId { get; set; }
        public string ?BloodBankName { get; set; }
        public string ?ContactNumber { get; set; }
        public string ?Email { get; set; }
        public int? StateId { get; set; }
        public int? districtId {get; set; }
        public int? CityId { get; set; }
        public string ?Address { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public int? AdminId { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
