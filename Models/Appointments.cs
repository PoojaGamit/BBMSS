using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BBMSDATA1.Models
{
  public class Appointments
    {
        [Key]
        public int  Id { get; set; }
        public int DonorId { get; set; }
        public int BloodBankId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string ? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool HasDisease {get;set;}
        public string? DiseaseName { get; set; }
        public string? AdditionalNotes { get; set; }
       
    }
}
