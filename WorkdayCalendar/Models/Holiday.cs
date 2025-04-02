using System;
using System.ComponentModel.DataAnnotations;

namespace WorkdayCalendar.Models
{
    public class Holiday
    {
        [Key]
        public int Id { get; set; } //  Primary Key

        [Required]
        public DateTime Date { get; set; } //  Date of the holiday

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // Name of the holiday

        [Required]
        public bool IsRecurring { get; set; } //  Is the holiday recurring? 
    }
}
