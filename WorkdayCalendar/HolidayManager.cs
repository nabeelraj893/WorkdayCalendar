using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WorkdayCalendar.Models;

namespace WorkdayCalendar
{
    public class HolidayManager
    {
        private readonly AppDbContext _dbContext;

        // Constructor: Initializes the holiday manager and ensures default holidays are set up.
        public HolidayManager(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            InitializeHolidays();
        }

        /// <summary>
        /// Populates the database with default recurring holidays if none exist.
        /// Prevents redundant initialization.
        /// </summary>
        public void InitializeHolidays()
        {
            if (_dbContext.Holidays.Any()) return; //  Prevent redundant initialization

            var defaultHolidays = new List<Holiday>
            {
                new Holiday { Date = new DateTime(2000, 1, 1), Name = "New Year’s Day", IsRecurring = true },
                new Holiday { Date = new DateTime(2000, 5, 1), Name = "Labour Day", IsRecurring = true },
                new Holiday { Date = new DateTime(2000, 5, 17), Name = "Constitution Day", IsRecurring = true },
                new Holiday { Date = new DateTime(2000, 12, 25), Name = "Christmas Day", IsRecurring = true },
                new Holiday { Date = new DateTime(2000, 12, 26), Name = "Boxing Day", IsRecurring = true },
                new Holiday { Date = new DateTime(2004, 5, 27), Name = "Special Holiday", IsRecurring = false }
            };

            _dbContext.Holidays.AddRange(defaultHolidays);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Adds a new holiday to the database if it does not already exist.
        /// </summary>
        public bool AddHoliday(DateTime date, bool isRecurring)
        {
            bool exists = isRecurring
                ? _dbContext.Holidays.Any(h => h.IsRecurring && h.Date.Month == date.Month && h.Date.Day == date.Day)
                : _dbContext.Holidays.Any(h => !h.IsRecurring && h.Date == date);

            if (exists)
            {
                Console.WriteLine(" Holiday already exists.");
                return false;
            }

            Console.Write("Enter holiday name: ");
            string name = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine(" Holiday name cannot be empty!");
                return false;
            }

            _dbContext.Holidays.Add(new Holiday { Date = date, Name = name, IsRecurring = isRecurring });
            _dbContext.SaveChanges();
            return true;
        }

        /// <summary>
        /// Removes a specified holiday from the database.
        /// </summary>
        public void RemoveHoliday(DateTime holiday, bool isRecurring)
        {
            var holidaysToRemove = _dbContext.Holidays
                .Where(h => h.IsRecurring == isRecurring &&
                            (isRecurring ? (h.Date.Month == holiday.Month && h.Date.Day == holiday.Day) : h.Date.Date == holiday.Date))
                .ToList();

            if (holidaysToRemove.Any())
            {
                _dbContext.Holidays.RemoveRange(holidaysToRemove);
                _dbContext.SaveChanges();
            }
            else
            {
                Console.WriteLine(" Holiday not found.");
            }
        }

        /// <summary>
        /// Retrieves all holidays for a given year, adjusting for recurring holidays.
        /// </summary>
        public List<Holiday> GetHolidays(int year)
        {
            return _dbContext.Holidays
                .Select(h => new Holiday
                {
                    Date = h.IsRecurring ? new DateTime(year, h.Date.Month, h.Date.Day) : h.Date,
                    Name = h.Name,
                    IsRecurring = h.IsRecurring
                })
                .ToList();
        }
    }
}
