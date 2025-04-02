using System;
using System.Globalization;
using System.Linq;
using WorkdayCalendar.Models;

namespace WorkdayCalendar
{
    internal class UserInterface
    {
        private readonly WorkHourManager _workHourManager;
        private readonly HolidayManager _holidayManager;
        private readonly WorkdayCalculator _workdayCalculator;

        public UserInterface(WorkHourManager workHourManager, HolidayManager holidayManager, WorkdayCalculator workdayCalculator)
        {
            _workHourManager = workHourManager;
            _holidayManager = holidayManager;
            _workdayCalculator = workdayCalculator;
        }

        public void Start()
        {
            while (true)
            {
                // Show the main menu
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n=================================");
                Console.WriteLine("          Workday Calendar        ");
                Console.WriteLine("=================================");
                Console.ResetColor();

                // Show the 4 menu options directly before asking for input
                Console.WriteLine("1. Calculate Workday");
                Console.WriteLine("2. Manage Work Hours");
                Console.WriteLine("3. Manage Holidays");
                Console.WriteLine("4. Exit");

                // Get user choice and highlight the selected option
                string choice = GetValidInput("Enter your choice: ", input => input == "1" || input == "2" || input == "3" || input == "4", " Invalid choice. Please enter 1, 2, 3, or 4.");
                HighlightSelectedOption(choice);

                // Switch to handle valid choices
                switch (choice)
                {
                    case "1":
                        // Handle Calculate Workday feature
                        WorkdayCalculatorCLI.Calculate(_workdayCalculator);
                        Console.WriteLine("\nWorkday calculation completed.");
                        break;
                    case "2":
                        // Handle Manage Work Hours feature
                        WorkHourManagerCLI.ManageWorkHours(_workHourManager);
                        break;
                    case "3":
                        // Handle Manage Holidays feature
                        HolidayManagerCLI.ManageHolidays(_holidayManager);
                        break;
                    case "4":
                        // Exit the application
                        Console.WriteLine("Are you sure you want to exit? (Y/N)");                      
                        string exitChoice;
                        // Input validation loop for exit confirmation
                        do
                        {
                            exitChoice = Console.ReadLine().Trim().ToLower(); // Ensure case-insensitive comparison

                            if (exitChoice != "y" && exitChoice != "n")
                            {
                                // Inform the user if input is invalid
                                Console.WriteLine("Invalid input. Please enter 'Y' to exit or 'N' to cancel.");
                            }
                        }
                        while (exitChoice != "y" && exitChoice != "n");

                        // Proceed with exit or return to menu based on user input
                        if (exitChoice == "y")
                        {
                            Console.WriteLine("Exiting... Goodbye!");
                            return; // Exit the program
                        }
                        else
                        {
                            Console.WriteLine("Returning to the main menu...");
                        }
                        break;
                }

                // After completing the action, prompt for returning to main menu
                Console.WriteLine("\nPress Enter to return to the main menu...");
                Console.ReadLine();
            }
        }

        // Helper method for input validation
        private static string GetValidInput(string prompt, Func<string, bool> validationFunc, string errorMessage)
        {
            string input;
            while (true)
            {
                Console.Write(prompt);
                input = Console.ReadLine().Trim();
                if (validationFunc(input)) break;
                Console.WriteLine(errorMessage);
            }
            return input;
        }

        // Method to highlight the selected option in the menu
        private void HighlightSelectedOption(string choice)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n=================================");
            Console.WriteLine("          Workday Calendar        ");
            Console.WriteLine("=================================");
            Console.ResetColor();

            // Loop through the menu options and highlight the selected one
            for (int i = 1; i <= 4; i++)
            {
                // Highlight the selected option
                if (choice == i.ToString())  
                {
                    Console.ForegroundColor = ConsoleColor.Yellow; 
                }

                Console.WriteLine($"{i}. {GetMenuOptionText(i)}");
                Console.ResetColor(); 
            }
            Console.Write("\nProcessing...\n");
            System.Threading.Thread.Sleep(500); 
        }

        // Method to get the text of the menu options
        private string GetMenuOptionText(int option)
        {
            return option switch
            {
                1 => "Calculate Workday",
                2 => "Manage Work Hours",
                3 => "Manage Holidays",
                4 => "Exit",
                _ => ""
            };
        }

        internal static class WorkdayCalculatorCLI
        {
            public static void Calculate(WorkdayCalculator workdayCalculator)
            {
                while (true) 
                {
                    DateTime startDate = GetValidDateTime("Enter start date (dd-MM-yyyy HH:mm, 24-hour format): ", " Invalid format. Please use 'dd-MM-yyyy HH:mm'.");

                    double workdays = GetValidWorkdays("Enter workdays to add/subtract (e.g., 2.5 for 2.5 days, -1 for subtracting): ", " Invalid number. Please enter a numeric value.");

                    // Perform calculation
                    DateTime resultDate = workdayCalculator.CalculateWorkday(startDate, workdays);
                    Console.WriteLine($" Resulting date: {resultDate:dd-MM-yyyy HH:mm}");

                    // Validate user's choice (Y/N)
                    string choice = GetValidInput("\nWould you like to perform another calculation? (Y/N): ", input => input == "y" || input == "n", " Invalid choice. Please enter 'Y' for Yes or 'N' for No.");

                    if (choice == "n")
                    {
                        return; // Exit and return to main menu
                    }
                }
            }

            private static DateTime GetValidDateTime(string prompt, string errorMessage)
            {
                DateTime startDate;
                while (true)
                {
                    Console.Write(prompt);
                    string input = Console.ReadLine().Trim();
                    if (DateTime.TryParseExact(input, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
                    {
                        break;
                    }
                    Console.WriteLine(errorMessage);
                }
                return startDate;
            }

            private static double GetValidWorkdays(string prompt, string errorMessage)
            {
                double workdays;
                while (true)
                {
                    Console.Write(prompt);
                    if (double.TryParse(Console.ReadLine().Trim(), out workdays) && workdays != 0)
                    {
                        break;
                    }
                    Console.WriteLine(errorMessage);
                }
                return workdays;
            }
        }

        internal static class WorkHourManagerCLI
        {
            public static void ManageWorkHours(WorkHourManager workHourManager)
            {
                while (true)
                {
                    Console.WriteLine("\n Manage Work Hours");
                    Console.WriteLine("1️ Set Work Hours");
                    Console.WriteLine("2️ View Work Hours");
                    Console.WriteLine("3️ Back to Main Menu");
                    string choice = GetValidInput("Enter choice: ", input => input == "1" || input == "2" || input == "3", " Invalid choice.");

                    switch (choice)
                    {
                        case "1":
                            SetWorkHours(workHourManager);
                            break;
                        case "2":
                            workHourManager.DisplayWorkHours();
                            break;
                        case "3":
                            return;
                    }
                }
            }

            private static void SetWorkHours(WorkHourManager workHourManager)
            {
                TimeSpan start = GetValidTime("Enter start hour (HH:mm, default 08:00): ");
                TimeSpan end = GetValidTime("Enter end hour (HH:mm, default 16:00): ", start);
                workHourManager.SetWorkHours(start, end);
            }

            private static TimeSpan GetValidTime(string message)
            {
                return GetValidTime(message, null);
            }

            private static TimeSpan GetValidTime(string message, TimeSpan? startTime)
            {
                while (true)
                {
                    Console.Write(message);
                    string input = Console.ReadLine().Trim();
                    if (TimeSpan.TryParseExact(input, "hh\\:mm", CultureInfo.InvariantCulture, out TimeSpan time))
                    {
                        if (startTime.HasValue && time <= startTime.Value)
                        {
                            Console.WriteLine(" End time must be after start time.");
                            continue;
                        }
                        return time;
                    }
                    Console.WriteLine(" Invalid format! Please enter time in HH:mm format.");
                }
            }
        }

        internal static class HolidayManagerCLI
        {
            public static void ManageHolidays(HolidayManager holidayManager)
            {
                while (true)
                {
                    Console.WriteLine("\ud83d\udd27 Manage Holidays");
                    Console.WriteLine("1\ufe0f\u20e3 Add Holiday");
                    Console.WriteLine("2\ufe0f\u20e3 Remove Holiday");
                    Console.WriteLine("3\ufe0f\u20e3 Show Holidays");
                    Console.WriteLine("4\ufe0f\u20e3 Back to Main Menu");
                    string choice = GetValidInput("Enter choice: ", input => input == "1" || input == "2" || input == "3" || input == "4", " Invalid choice.");

                    switch (choice)
                    {
                        case "1":
                            AddHoliday(holidayManager);
                            break;
                        case "2":
                            RemoveHoliday(holidayManager);
                            break;
                        case "3":
                            ShowHolidays(holidayManager);
                            break;
                        case "4":
                            return;
                    }
                }
            }

            private static void AddHoliday(HolidayManager holidayManager)
            {
                while (true)
                {
                    DateTime newHoliday = GetValidDateTime("Enter holiday date (dd-MM-yyyy): ", " Invalid date format. Please enter a valid date.");
                    bool isRecurring = GetValidInput("Is this a recurring holiday? (yes/no): ", input => input == "yes" || input == "no", " Please enter 'yes' or 'no'.") == "yes";

                    if (holidayManager.AddHoliday(newHoliday, isRecurring))
                    {
                        Console.WriteLine($"\u2705 Holiday added: {newHoliday:dd-MM-yyyy} ({(isRecurring ? "Recurring" : "Fixed")})");
                    }
                    return; // Exit function after successful addition
                }
            }

            private static void RemoveHoliday(HolidayManager holidayManager)
            {
                DateTime removeHoliday = GetValidDateTime("Enter holiday to remove (dd-MM-yyyy): ", " Invalid date format. Please enter in 'dd-MM-yyyy' format.");

                var holidays = holidayManager.GetHolidays(removeHoliday.Year);
                var exactMatch = holidays.FirstOrDefault(h => h.Date == removeHoliday && !h.IsRecurring);
                var recurringMatch = holidays.FirstOrDefault(h => h.Date.Month == removeHoliday.Month && h.Date.Day == removeHoliday.Day && h.IsRecurring);

                if (exactMatch != null)
                {
                    holidayManager.RemoveHoliday(removeHoliday, false);
                    Console.WriteLine($"\u2705 Holiday removed: {removeHoliday:dd-MM-yyyy} (Fixed)");
                }
                else if (recurringMatch != null)
                {
                    holidayManager.RemoveHoliday(removeHoliday, true);
                    Console.WriteLine($"\u2705 Holiday removed: {removeHoliday:dd-MM-yyyy} (Recurring)");
                }
                else
                {
                    Console.WriteLine("\u26a0 Holiday not found.");
                }
            }

            private static void ShowHolidays(HolidayManager holidayManager)
            {
                Console.WriteLine("\ud83d\udcc5 Holidays:");
                var holidaysList = holidayManager.GetHolidays(DateTime.Now.Year);

                if (holidaysList.Count == 0)
                {
                    Console.WriteLine("No holidays set.");
                }
                else
                {
                    foreach (var holiday in holidaysList)
                    {
                        Console.WriteLine($"{holiday.Date:dd-MM-yyyy} ({(holiday.IsRecurring ? "Recurring" : "Fixed")})");
                    }
                }
            }

            private static DateTime GetValidDateTime(string prompt, string errorMessage)
            {
                DateTime date;
                while (true)
                {
                    Console.Write(prompt);
                    string input = Console.ReadLine().Trim();
                    if (DateTime.TryParseExact(input, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                    {
                        break;
                    }
                    Console.WriteLine(errorMessage);
                }
                return date;
            }
        }
    }
}

