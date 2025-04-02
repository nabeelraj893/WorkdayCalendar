using System;
using Xunit;
using Microsoft.EntityFrameworkCore;
using WorkdayCalendar.Models;
using WorkdayCalendar;  

public class WorkdayCalendarTests
{
    private readonly WorkdayCalculator _calculator;

    public WorkdayCalendarTests()
    {
        // Set up in-memory database for testing
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "WorkdayCalculatorTestDatabase")
            .Options;

        var dbContext = new AppDbContext(options);

        SeedTestData(dbContext);
        var workHourManager = new WorkHourManager(dbContext);
        var holidayManager = new HolidayManager(dbContext);

        _calculator = new WorkdayCalculator(workHourManager, holidayManager);
    }

    private void SeedTestData(AppDbContext dbContext)
    {
        dbContext.Holidays.AddRange(new[]
        {
        new Holiday { Name = "Fixed Holiday", Date = new DateTime(2004, 5, 27), IsRecurring = false },
        new Holiday { Name = "Recurring Holiday", Date = new DateTime(2004, 5, 17), IsRecurring = true }
    });

        dbContext.WorkHours.AddRange(new[]
        {
        new WorkHours { StartTime = TimeSpan.FromHours(8), EndTime = TimeSpan.FromHours(16) },
        new WorkHours { StartTime = TimeSpan.FromHours(8), EndTime = TimeSpan.FromHours(16) },
        new WorkHours { StartTime = TimeSpan.FromHours(8), EndTime = TimeSpan.FromHours(16) },
        new WorkHours { StartTime = TimeSpan.FromHours(8), EndTime = TimeSpan.FromHours(16) },
        new WorkHours { StartTime = TimeSpan.FromHours(8), EndTime = TimeSpan.FromHours(16) }
    });

        dbContext.SaveChanges();
    }

    [Fact]
    public void Test_AddWorkdays_BasicCase()
    {
        DateTime startDate = new DateTime(2004, 5, 24, 15, 7, 0);
        double workdaysToAdd = 0.25;
        DateTime result = _calculator.CalculateWorkday(startDate, workdaysToAdd);
        DateTime expected = new DateTime(2004, 5, 25, 9, 7, 0);
        Assert.True(Math.Abs((expected - result).TotalMinutes) < 1,
            $"Expected {expected}, but got {result}");
    }

    [Fact]
    public void Test_AddWorkdays_MidnightBoundary()
    {
        DateTime startDate = new DateTime(2004, 5, 24, 4, 0, 0);
        double workdaysToAdd = 0.5;
        DateTime result = _calculator.CalculateWorkday(startDate, workdaysToAdd);
        DateTime expected = new DateTime(2004, 5, 24, 12, 0, 0);
        Assert.True(Math.Abs((expected - result).TotalMinutes) < 1,
            $"Expected {expected}, but got {result}");
    }
    [Fact]
    public void Test_SubtractWorkdays_WithHolidays()
    {
        DateTime startDate = new DateTime(2004, 5, 24, 18, 5, 0);
        double workdaysToSubtract = -5.5;

        DateTime result = _calculator.CalculateWorkday(startDate, workdaysToSubtract);
        DateTime expected = new DateTime(2004, 5, 14, 12, 0, 0);
        Assert.True(Math.Abs((expected - result).TotalMinutes) < 1,
            $"Expected {expected}, but got {result}");
    }

    [Fact]
    public void Test_AddWorkdays_LargeCase()
    {
        DateTime startDate = new DateTime(2004, 5, 24, 19, 3, 0);
        double workdaysToAdd = 44.723656;
        DateTime result = _calculator.CalculateWorkday(startDate, workdaysToAdd);
        DateTime expected = new DateTime(2004, 7, 27, 13, 50, 0);
        Assert.True(Math.Abs((expected - result).TotalMinutes) < 1,
            $"Expected {expected}, but got {result}");
    }

    [Fact]
    public void Test_SubtractWorkdays_LargeCase()
    {
        DateTime startDate = new DateTime(2004, 5, 24, 18, 3, 0);
        double workdaysToSubtract = -6.7470217;
        DateTime result = _calculator.CalculateWorkday(startDate, workdaysToSubtract);
        DateTime expected = new DateTime(2004, 5, 13, 10, 1, 0);
        Assert.True(Math.Abs((expected - result).TotalMinutes) < 1,
            $"Expected {expected}, but got {result}");
    }

    [Fact]
    public void Test_AddWorkdays_PreciseCalculation1()
    {
        DateTime startDate = new DateTime(2004, 5, 24, 8, 3, 0);
        double workdaysToAdd = 12.782709;

        DateTime result = _calculator.CalculateWorkday(startDate, workdaysToAdd);
        DateTime expected = new DateTime(2004, 6, 10, 14, 18, 0); 
        Assert.True(Math.Abs((expected - result).TotalMinutes) < 1,
            $"Expected {expected}, but got {result}");
    }

    [Fact]
    public void Test_AddWorkdays_PreciseCalculation2()
    {
        DateTime startDate = new DateTime(2004, 5, 24, 7, 3, 0);
        double workdaysToAdd = 8.276628;

        DateTime result = _calculator.CalculateWorkday(startDate, workdaysToAdd);
        DateTime expected = new DateTime(2004, 6, 4, 10, 15, 0);
        Assert.True(Math.Abs((expected - result).TotalMinutes) < 1,
            $"Expected {expected}, but got {result}");
    }
}
