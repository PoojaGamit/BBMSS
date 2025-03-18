
using System.ComponentModel.DataAnnotations;
using DocumentFormat.OpenXml.Drawing;


namespace BBMSDATA1.Models
{
   public class Donorregi
    {
        [Key]
        public int DId { get; set; }
        [Required]
        public string ?Name { get; set; }
        [Required]
        public  string ?Gender { get; set; }

        [Required]
        public int Age { get; set; }
        [Required]
        public string ?Mobile { get; set; }
        [Required]
        public int Bloodgroupid { get; set; }
        [Required]
        public string ?Address { get; set; }
        [Required]
        public int Stateid { get; set; }
        [Required]
        public int Districtid { get; set; }
        [Required]
        public int Cityid { get; set; }
        [Required]
        public int BloodBankid { get; set; }
        [Required]
        public DateTime Tentativedate { get; set; }=DateTime.Now;

        public bool PreviouslyDonated { get; set; }
         public bool HistoryOfSurgeryOrTransfusion { get; set; }

    }
       
}
