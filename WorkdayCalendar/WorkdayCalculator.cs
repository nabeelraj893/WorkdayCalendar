using System;
using System.Collections.Generic;
using System.Linq;

namespace WorkdayCalendar
{
    public class WorkdayCalculator
    {
        private readonly WorkHourManager _workHourManager;
        private readonly HolidayManager _holidayManager;
        private static readonly double WorkHoursPerDay = 8.0;

        public TimeSpan WorkdayStart => _workHourManager.WorkdayStart;
        public TimeSpan WorkdayEnd => _workHourManager.WorkdayEnd;

        public WorkdayCalculator(WorkHourManager workHourManager, HolidayManager holidayManager)
        {
            _workHourManager = workHourManager;
            _holidayManager = holidayManager;
        }

        /// <summary>
        /// Calculates the target workday by adding or subtracting the specified number of workdays from the start date.
        /// </summary>
        public DateTime CalculateWorkday(DateTime start, double workdays)
        {
            return workdays >= 0 ? AddWorkdays(start, workdays) : SubtractWorkdays(start, -workdays);
        }

        /// <summary>
        /// Adds the specified number of workdays to the given start date, considering weekends and holidays.
        /// </summary>
        private DateTime AddWorkdays(DateTime start, double workdays)
        {
            DateTime currentTime = start;
            // Preserve the original minutes
            int originalMinutes = currentTime.Minute;
            //  Ensure we start within working hours
            if (currentTime.TimeOfDay < WorkdayStart)
            {
                currentTime = currentTime.Date.Add(WorkdayStart).AddMinutes(originalMinutes);
            }
            else if (currentTime.TimeOfDay >= WorkdayEnd)
            {
                currentTime = GetNextWorkdayStart(currentTime).AddMinutes(originalMinutes);
            }
            double remainingHours = workdays * WorkHoursPerDay;
            while (remainingHours > 0)
            {
                double availableHours = (WorkdayEnd - currentTime.TimeOfDay).TotalHours;
                if (remainingHours <= availableHours)
                {
                    //currentTime = currentTime.AddHours(remainingHours);
                    currentTime = currentTime.AddMinutes(Math.Floor(remainingHours * 60));
                    //  Ensure precise minute addition
                    remainingHours = 0;
                }
                else
                {
                    remainingHours -= availableHours;
                    currentTime = GetNextWorkdayStart(currentTime.Date.AddDays(1)); // Ensure correct reset
                }
            }
            return currentTime;
        }

        /// <summary>
        /// Subtracts the specified number of workdays from the given start date, considering weekends and holidays.
        /// </summary>
        private DateTime SubtractWorkdays(DateTime start, double workdays)
        {
            DateTime currentTime = start;

            if (currentTime.TimeOfDay >= WorkdayEnd)
            {
                currentTime = currentTime.Date.Add(WorkdayEnd);
            }
            else if (currentTime.TimeOfDay < WorkdayStart)
            {
                currentTime = GetPreviousWorkdayEnd(currentTime.Date.AddDays(-1));
            }

            while (workdays > 0)
            {
                //  Ensure we start from a valid workday
                while (IsHolidayOrWeekend(currentTime))
                {
                    currentTime = GetPreviousWorkdayEnd(currentTime.Date.AddDays(-1));
                }

                double availableHours = (currentTime.TimeOfDay - WorkdayStart).TotalHours;

                if (workdays * WorkHoursPerDay <= availableHours)
                {
                    currentTime = currentTime.AddHours(-workdays * WorkHoursPerDay);
                    break;
                }
                else
                {
                    workdays -= availableHours / WorkHoursPerDay;
                    currentTime = GetPreviousWorkdayEnd(currentTime.Date.AddDays(-1));
                }
            }

            return currentTime;
        }

        /// <summary>
        /// Finds the next valid workday start time, skipping holidays and weekends.
        /// </summary>
        private DateTime GetNextWorkdayStart(DateTime dateTime)
        {

            if (dateTime.TimeOfDay >= WorkdayEnd || IsHolidayOrWeekend(dateTime))
            {
                dateTime = dateTime.AddDays(1);
                while (IsHolidayOrWeekend(dateTime))
                {
                    dateTime = dateTime.AddDays(1);
                }
                dateTime = dateTime.Date.Add(WorkdayStart);
            }
            else if (dateTime.TimeOfDay < WorkdayStart)
            {
                dateTime = dateTime.Date.Add(WorkdayStart); 
            }
            while (IsHolidayOrWeekend(dateTime))
            {
                dateTime = dateTime.AddDays(1).Date.Add(WorkdayStart);

            }
            return dateTime; 
        }

        /// <summary>
        /// Finds the previous valid workday end time, skipping holidays and weekends.
        /// </summary>
        private DateTime GetPreviousWorkdayEnd(DateTime dateTime)
        {
            while (IsHolidayOrWeekend(dateTime.Date))
            {
                dateTime = dateTime.AddDays(-1); // Move back one full day
            }
            return dateTime.Date.Add(WorkdayEnd);
        }

        /// <summary>
        /// Checks if a given date is a weekend or a holiday.
        /// </summary>
        private bool IsHolidayOrWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday ||
                   _holidayManager.GetHolidays(date.Year)
                       .Any(h => h.Date.Date == date.Date || (h.IsRecurring && h.Date.Month == date.Month && h.Date.Day == date.Day));
        }

    }
}



