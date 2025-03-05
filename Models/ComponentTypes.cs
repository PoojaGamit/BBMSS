using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBMSDATA1.Models
{
  public class ComponentTypes
    {
        [Key]
        public int ComponentTypeId { get; set; }
        public string ?ComponentTypeName { get; set; }

    }
}
