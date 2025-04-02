# Workday Calendar

## Project Overview  

The **Workday Calendar** is an application designed for **Supply Chain Management**, enabling accurate calculation of working days while considering **weekends, holidays, and specific work hours**. This ensures precise scheduling for **production planning** by determining:  

- **Critical order dates** for supplier components.  
- **Anticipated delivery dates** for final products, factoring in **production and shipping durations**.  

###  Key Features  
- **Customizable Work Hours**  Define flexible work schedules.  
- **Holiday Management**  Supports **both recurring and one-time holidays**.  
- **Workday Calculations**  Add and subtract **fractional working days** while considering non-working periods.  

This application helps businesses **streamline their supply chain scheduling**, ensuring that production timelines align accurately with supplier lead times and holidays.   


##  Installation & Setup

###  Prerequisites
Ensure you have the following installed:
- **.NET 8.0 SDK**  [Download Here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- **SQL Server** (Express or Full)  [Download Here](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- **Visual Studio** (with `.NET` & `SQL Server` development workloads)

###  Clone the Repository
```sh
git clone https://github.com/yourusername/WorkdayCalendar.git
cd WorkdayCalendar
```

###  Configure the Database
1. **Update the connection string** in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=WorkdayCalendarDB;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```
   > Replace **YOUR_SERVER_NAME** with your actual SQL Server instance.

2. **Apply Migrations & Seed Data**:
   ```sh
   dotnet ef database update
   ```
   This will create the database and populate default holidays & work hours.

###  Install Dependencies
Run the following command to install required NuGet packages:
```sh
dotnet restore
```

###  Run the Application
To start the Workday Calendar CLI:
```sh
dotnet run --project WorkdayCalendar
```

---

##  Usage Examples
###  Add a Workday
```
Enter start date: 24.05.2004 15:07
Enter workday increment: 0.25
Result: 25.05.2004 09:07
```

###  Subtract Workdays
```
Enter start date: 24.05.2004 18:05
Enter workday decrement: -5.5
Result: 14.05.2004 12:00
```

---

##  Running Tests
To execute unit tests, run:
```sh
dotnet test
```
###  Testing Frameworks Used:
- **xUnit** (for unit tests)
- **Moq** (for mocking dependencies)

---

##  Technologies Used
- **.NET 8.0**
- **C#**
- **Entity Framework Core (SQL Server)**
- **xUnit (Unit Testing)**
- **Moq (Mocking)**
- **Newtonsoft.Json**

---


## Contact
For any questions or clarifications, please reach out to [rajanabeelahsan.bce@example.com].

---

>>>>>>> f6bd8b7 (Initial commit - Workday Calendar project)
