using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.IO;
using WorkdayCalendar;

class Program
{
    static void Main(string[] args)
    {
        // Build and configure the host (dependency injection container)
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders(); // Remove all logging
                logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.None); // Suppress EF Core logs
            })
            .ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                string connectionString = context.Configuration.GetConnectionString("DefaultConnection");

                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(connectionString)
                           .EnableSensitiveDataLogging(false) //  No extra data logs
                           .LogTo(Console.WriteLine, LogLevel.None)); //  Completely disable logs

                services.AddSingleton<WorkHourManager>();
                services.AddSingleton<HolidayManager>();
                services.AddSingleton<WorkdayCalculator>();
            })
            .Build();

        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            // Retrieve the AppDbContext from the service provider
            var dbContext = services.GetRequiredService<AppDbContext>();
            dbContext.Database.EnsureCreated();

            var workHourManager = services.GetRequiredService<WorkHourManager>();
            var holidayManager = services.GetRequiredService<HolidayManager>();
            var workdayCalculator = services.GetRequiredService<WorkdayCalculator>();

            // Create and start the user interface with the required services

            var userInterface = new UserInterface(workHourManager, holidayManager, workdayCalculator);
            userInterface.Start();
        }
        catch (Exception)
        {
            // Suppress error messages for a clean user interface
        }
    }
}

