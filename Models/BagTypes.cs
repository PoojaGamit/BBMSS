using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBMSDATA1.Models
{
   public class BagTypes
    {
        [Key]
        public int BagTypeId { get; set; }
        public string ?BagTypeName { get; set; }

    }
}
