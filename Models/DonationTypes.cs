using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBMSDATA1.Models
{
    public class DonationTypes
    {
        [Key]
        public int DonationTypeId { get; set; }
        public string ?DonationTypeName { get; set; }

        
    }
}
