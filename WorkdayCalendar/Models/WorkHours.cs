using System;

namespace WorkdayCalendar.Models
{
    public class WorkHours
    {
        public int Id { get; set; }  //  Primary Key
        public TimeSpan StartTime { get; set; }  //  Start Time
        public TimeSpan EndTime { get; set; }    //  End Time
    }
}
