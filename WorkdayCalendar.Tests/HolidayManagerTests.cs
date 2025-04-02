using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WorkdayCalendar.Models;
using WorkdayCalendar;
using Xunit;

namespace WorkdayCalendar.Tests
{
    public class HolidayManagerTests
    {
        private readonly HolidayManager _holidayManager;
        private readonly AppDbContext _dbContext;

        // Constructor to set up the in-memory database and initialize the HolidayManager
        public HolidayManagerTests()
        {
            // Configure in-memory database options for testing
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new AppDbContext(options);
            _holidayManager = new HolidayManager(_dbContext);
        }

        // Helper method to clear existing holiday data from the database
        private void ClearData()
        {
            _dbContext.Holidays.RemoveRange(_dbContext.Holidays);
            _dbContext.SaveChanges();
            Assert.Empty(_dbContext.Holidays);
        }

        // Helper method to seed test data into the holiday table
        private void SeedTestData()
        {
            _dbContext.Holidays.AddRange(new[]
            {
                new Holiday { Date = new DateTime(2000, 1, 1), Name = "New Year’s Day", IsRecurring = true },
                new Holiday { Date = new DateTime(2000, 5, 1), Name = "Labour Day", IsRecurring = true },
                new Holiday { Date = new DateTime(2000, 5, 17), Name = "Constitution Day", IsRecurring = true },
                new Holiday { Date = new DateTime(2000, 12, 25), Name = "Christmas Day", IsRecurring = true },
                new Holiday { Date = new DateTime(2000, 12, 26), Name = "Boxing Day", IsRecurring = true },
                new Holiday { Date = new DateTime(2004, 5, 27), Name = "Special Holiday", IsRecurring = false }
            });
            _dbContext.SaveChanges();
        }

        [Fact]
        public void Test_InitializeHolidays()
        {
            ClearData();
            _holidayManager.InitializeHolidays();
            var holidays = _dbContext.Holidays.ToList();
            Assert.Equal(6, holidays.Count);
        }

        // Test the adding of a new holiday
        [Fact]
        public void Test_AddHoliday()
        {
            ClearData();
            SeedTestData();

            // Simulate user input for the holiday name
            var input = new StringReader("Test Holiday\n");
            Console.SetIn(input);

            var result = _holidayManager.AddHoliday(new DateTime(2024, 7, 4), false);
            Assert.True(result, "Expected AddHoliday to return True, but got False");

            var addedHoliday = _dbContext.Holidays.FirstOrDefault(h => h.Date == new DateTime(2024, 7, 4));
            Assert.NotNull(addedHoliday);
            Assert.Equal("Test Holiday", addedHoliday.Name);
        }

        // Test adding a duplicate recurring holiday
        [Fact]
        public void Test_AddDuplicateRecurringHoliday()
        {
            ClearData();
            SeedTestData();

            var input = new StringReader("Duplicate Holiday\n");
            Console.SetIn(input);

            var result = _holidayManager.AddHoliday(new DateTime(2000, 1, 1), true);
            Assert.False(result, "Expected AddHoliday to return False for duplicate holiday");
        }

        // Test removing an existing holiday
        [Fact]
        public void Test_RemoveHoliday()
        {
            ClearData();
            SeedTestData();

            _holidayManager.RemoveHoliday(new DateTime(2000, 1, 1), true);
            var holiday = _dbContext.Holidays.FirstOrDefault(h => h.Date == new DateTime(2000, 1, 1));
            Assert.Null(holiday);
        }

        // Test retrieving holidays for a given year
        [Fact]
        public void Test_GetHolidays()
        {
            ClearData();
            SeedTestData();

            var holidays = _holidayManager.GetHolidays(2000);
            Assert.Equal(6, holidays.Count);
        }
    }
}
