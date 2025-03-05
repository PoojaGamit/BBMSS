using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BBMSDATA1.Models
{
  public class Users
    {
        [Key]
        public int UserId { get; set; }
        public string ?FullName { get; set; }
        public string ?Email { get; set; }
        public string ?PasswordHash { get; set; }
        public string ?PhoneNumber { get; set; }
      
        public int? BloodGroupId { get; set; }
        //public int? CityId { get; set; }
        public int? StateId { get; set; }
        public string ?Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool IsActive { get; set; } = true;
        public string? PinCode { get; set; }
        public int R_ID { get; set; }
    
    }
}
