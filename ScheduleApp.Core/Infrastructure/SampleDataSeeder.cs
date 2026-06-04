using Microsoft.EntityFrameworkCore;
using ScheduleAppCore.Data;
using ScheduleAppCore.Models;

namespace ScheduleAppCore.Infrastructure;

public static class SampleDataSeeder
{
    public static void Seed(ScheduleContext context)
    {
        var sampleLastNames = new HashSet<string>
        {
            "Bukielski",
            "Czajewicz",
            "Czarkowska",
            "Czekaj",
            "Kalbierz",
            "Kanialska",
            "Komorek",
            "Kostrzewski"
        };

        if (context.Employees.Any(employee => sampleLastNames.Contains(employee.LastName)))
        {
            return;
        }

        if (context.ScheduleEntries.Any())
        {
            context.ScheduleEntries.RemoveRange(context.ScheduleEntries);
        }

        if (context.Employees.Any())
        {
            context.Employees.RemoveRange(context.Employees);
        }

        context.SaveChanges();

        var employees = new List<Employee>
        {
            new() { FirstName = "Andrzej", LastName = "Bukielski", NormHours = 160, NormDays = 20 },
            new() { FirstName = "Ola", LastName = "Czajewicz", NormHours = 160, NormDays = 20 },
            new() { FirstName = "Ewa", LastName = "Czarkowska", NormHours = 160, NormDays = 20 },
            new() { FirstName = "Katarzyna", LastName = "Czekaj", NormHours = 160, NormDays = 20 },
            new() { FirstName = "Igor", LastName = "Kalbierz", NormHours = 160, NormDays = 20 },
            new() { FirstName = "Małgorzata", LastName = "Kanialska", NormHours = 160, NormDays = 20 },
            new() { FirstName = "Joanna", LastName = "Komorek", NormHours = 160, NormDays = 20 },
            new() { FirstName = "Adam", LastName = "Kostrzewski", NormHours = 160, NormDays = 20 }
        };

        context.Employees.AddRange(employees);
        context.SaveChanges();

        var monthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var monthEnd = monthStart.AddMonths(1);

        foreach (var employee in employees)
        {
            for (var date = monthStart.Date; date < monthEnd.Date; date = date.AddDays(1))
            {
                if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                {
                    continue;
                }

                var type = "Normal";
                decimal? startHour = 7;
                decimal? endHour = 15;
                string? notes = null;

                if (employee.LastName == "Kalbierz" && date.DayOfWeek is DayOfWeek.Tuesday or DayOfWeek.Thursday or DayOfWeek.Friday)
                {
                    type = "Noc";
                }

                if (employee.LastName is "Bukielski" or "Czajewicz" or "Czarkowska" or "Czekaj" or "Kanialska" or "Komorek" &&
                    date == new DateTime(monthStart.Year, monthStart.Month, 26))
                {
                    type = "Wolne";
                    startHour = null;
                    endHour = null;
                    notes = "W5";
                }

                if (employee.LastName == "Bukielski" && date == new DateTime(monthStart.Year, monthStart.Month, 31))
                {
                    type = "Święto";
                    startHour = null;
                    endHour = null;
                }

                if (employee.LastName == "Kalbierz" && date == new DateTime(monthStart.Year, monthStart.Month, 29))
                {
                    type = "Noc";
                }

                if (employee.LastName == "Kalbierz" && date == new DateTime(monthStart.Year, monthStart.Month, 30))
                {
                    type = "Wolne";
                    startHour = null;
                    endHour = null;
                    notes = "W5";
                }

                if (employee.LastName == "Kalbierz" && date == new DateTime(monthStart.Year, monthStart.Month, 31))
                {
                    type = "Święto";
                    startHour = null;
                    endHour = null;
                }

                if (date.DayOfWeek == DayOfWeek.Saturday)
                {
                    type = "Wolne";
                    startHour = null;
                    endHour = null;
                    notes = "W5";
                }

                if (date.DayOfWeek == DayOfWeek.Sunday)
                {
                    type = "Święto";
                    startHour = null;
                    endHour = null;
                }

                if (employee.LastName == "Kalbierz" && date.DayOfWeek == DayOfWeek.Wednesday)
                {
                    type = "Normal";
                }

                context.ScheduleEntries.Add(new ScheduleEntry
                {
                    EmployeeId = employee.Id,
                    Date = date,
                    Type = type,
                    StartHour = startHour,
                    EndHour = endHour,
                    Notes = notes
                });
            }
        }

        context.SaveChanges();
    }
}