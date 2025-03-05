using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBMSDATA1.Models
{
    public class Roles
    {
        [Key]
        public int R_Id { get; set; }
        public string? R_Name {get; set; }
    }
}
