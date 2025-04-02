using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WorkdayCalendar.Models;

namespace WorkdayCalendar
{
    public class WorkHourManager
    {
        private readonly AppDbContext _dbContext;

        // Properties to store workday start and end times
        public TimeSpan WorkdayStart { get; private set; }
        public TimeSpan WorkdayEnd { get; private set; }

        // Constructor initializes the database context and loads work hours
        public WorkHourManager(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            LoadWorkHours();
        }

        // Loads work hours from the database or sets default values if none exist
        private void LoadWorkHours()
        {
            var workHours = _dbContext.WorkHours.FirstOrDefault();
            if (workHours != null)
            {
                // Assign stored values if available
                WorkdayStart = workHours.StartTime;
                WorkdayEnd = workHours.EndTime;
            }
            else
            {
                // Set default work hours (8:00 AM - 4:00 PM) if none exist
                WorkdayStart = new TimeSpan(8, 0, 0);
                WorkdayEnd = new TimeSpan(16, 0, 0);
                _dbContext.WorkHours.Add(new WorkHours { StartTime = WorkdayStart, EndTime = WorkdayEnd });
                _dbContext.SaveChanges();
            }
        }

        // Updates the work hours and saves the changes in the database
        public void SetWorkHours(TimeSpan start, TimeSpan end)
        {
            if (start >= end)
            {
                Console.WriteLine(" Invalid work hours! Start time must be before end time.");
                return;
            }

            var existingEntry = _dbContext.WorkHours.FirstOrDefault();
            if (existingEntry != null)
            {
                // Update existing record
                existingEntry.StartTime = start;
                existingEntry.EndTime = end;
            }
            else
            {
                // Create a new record if no existing entry found
                _dbContext.WorkHours.Add(new WorkHours { StartTime = start, EndTime = end });
            }

            _dbContext.SaveChanges();
            WorkdayStart = start;
            WorkdayEnd = end;
            Console.WriteLine($" Work hours updated: {WorkdayStart} to {WorkdayEnd}");
        }

        // Displays the current work hours
        public void DisplayWorkHours()
        {
            Console.WriteLine($" Current Work Hours: {WorkdayStart} to {WorkdayEnd}");
        }
    }
}
