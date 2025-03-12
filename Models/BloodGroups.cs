using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBMSDATA1.Models
{
   public class BloodGroups
    {
        [Key]
        public int BloodGroupId { get; set; }
        public string ?BloodGroupName { get; set; }

    }
}
