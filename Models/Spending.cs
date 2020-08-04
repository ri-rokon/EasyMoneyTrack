using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EasyMoneyTrack.Models
{
    public class Spending 
    {
        public int Id { get; set; }
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Withdraw ammount must be greater then 0")]
        public double Withdraw { get; set; }

        public DateTime Date { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual IdentityUser IdentityUser { get; set; }
    }
}
