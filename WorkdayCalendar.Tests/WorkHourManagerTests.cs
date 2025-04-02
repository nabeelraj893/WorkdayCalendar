using System;
using Microsoft.EntityFrameworkCore;
using WorkdayCalendar.Models;
using WorkdayCalendar;
using Xunit;

namespace WorkdayCalendar.Tests
{
    public class WorkHourManagerTests
    {
        private readonly WorkHourManager _workHourManager;
        private readonly AppDbContext _dbContext;

        public WorkHourManagerTests()
        {
            // Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "WorkHourManagerTestDatabase")
                .Options;

            _dbContext = new AppDbContext(options);

            // Initialize WorkHourManager with the in-memory database context
            _workHourManager = new WorkHourManager(_dbContext);
        }

        // Clear existing data to ensure consistency in tests
        private void ClearData()
        {
            _dbContext.WorkHours.RemoveRange(_dbContext.WorkHours);
            _dbContext.SaveChanges();
        }

        private void SeedTestData(AppDbContext dbContext)
        {
            // Seed data only when required (in each test individually)
            dbContext.WorkHours.AddRange(new[] {
                new WorkHours { StartTime = TimeSpan.FromHours(8), EndTime = TimeSpan.FromHours(16) },
            });
            dbContext.SaveChanges();
        }

        // Test: Ensure default work hours are set correctly
        [Fact]
        public void Test_GetDefaultWorkHours()
        {
            ClearData();
            SeedTestData(_dbContext); // Ensure that we have the expected seed data

            Assert.Equal(new TimeSpan(8, 0, 0), _workHourManager.WorkdayStart);
            Assert.Equal(new TimeSpan(16, 0, 0), _workHourManager.WorkdayEnd);
        }

        //  Test: Ensure work hours are loaded correctly from the database
        [Fact]
        public void Test_LoadWorkHoursFromDb()
        {
            ClearData();
            SeedTestData(_dbContext);

            var workHours = _dbContext.WorkHours.FirstOrDefault();

            // Ensure we are getting the work hours data from the in-memory database
            Assert.NotNull(workHours);
            Assert.Equal(new TimeSpan(8, 0, 0), workHours.StartTime);
            Assert.Equal(new TimeSpan(16, 0, 0), workHours.EndTime);
        }

        //  Test: Set custom work hours and ensure they are saved
        [Fact]
        public void Test_SetCustomWorkHours()
        {
            ClearData();
            SeedTestData(_dbContext);

            _workHourManager.SetWorkHours(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0));

            Assert.Equal(new TimeSpan(9, 0, 0), _workHourManager.WorkdayStart);
            Assert.Equal(new TimeSpan(17, 0, 0), _workHourManager.WorkdayEnd);
        }

        //  Test: Invalid work hours should not be set
        [Fact]
        public void Test_SetInvalidWorkHours()
        {
            ClearData();
            SeedTestData(_dbContext);

            _workHourManager.SetWorkHours(new TimeSpan(18, 0, 0), new TimeSpan(9, 0, 0));

            // Work hours should remain unchanged
            Assert.NotEqual(new TimeSpan(18, 0, 0), _workHourManager.WorkdayStart);
            Assert.NotEqual(new TimeSpan(9, 0, 0), _workHourManager.WorkdayEnd);
        }
    }
}
