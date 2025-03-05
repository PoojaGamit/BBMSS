using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBMSDATA1.Models
{
    public class Districts
    {
        [Key]
        public int Id { get; set; }
        public string ?DistrictName { get; set; }
        public int StateId { get; set; }

      
    }
}
