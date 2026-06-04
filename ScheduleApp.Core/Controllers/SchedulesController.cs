using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using ScheduleAppCore.Data;
using ScheduleAppCore.Models;

namespace ScheduleAppCore.Controllers;

public class SchedulesController : Controller
{
    private static readonly string[] EntryTypes = ["Normal", "Noc", "Wolne", "Święto", "Urlop", "Zwolnienie chorobowe"];

    private readonly ScheduleContext _context;

    public SchedulesController(ScheduleContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(DateTime? week, string? view, int? employeeId)
    {
        var viewMode = NormalizeViewMode(view);
        var referenceDate = week ?? DateTime.Today;
        var weekStart = GetViewStart(referenceDate, viewMode);
        var visibleDays = GetVisibleDays(weekStart, viewMode);

        var employees = await _context.Employees
            .OrderBy(employee => employee.LastName)
            .ThenBy(employee => employee.FirstName)
            .ToListAsync();

        var entries = await _context.ScheduleEntries
            .Include(entry => entry.Employee)
            .ToListAsync();

        var selectedEmployeeId = employeeId;
        if (viewMode == "employee" && selectedEmployeeId == null)
        {
            selectedEmployeeId = employees.FirstOrDefault()?.Id;
        }

        var model = new ScheduleViewModel
        {
            WeekStart = weekStart,
            ViewMode = viewMode,
            SelectedEmployeeId = selectedEmployeeId,
            VisibleDays = visibleDays,
            Employees = employees,
            Entries = entries
        };

        return View(model);
    }

    public async Task<IActionResult> Create(int? employeeId, DateTime? date)
    {
        var model = new ScheduleEntryEditViewModel
        {
            EmployeeId = employeeId,
            Date = date?.Date ?? DateTime.Today,
            Employees = await GetEmployeeItemsAsync(),
            TypeOptions = GetTypeOptions()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ScheduleEntryEditViewModel model)
    {
        // Try to parse StartHour/EndHour from form (handle locale decimal separator)
        if (string.IsNullOrEmpty(model?.StartHour.ToString()) && Request.HasFormContentType && Request.Form.ContainsKey("StartHour"))
        {
            var raw = Request.Form["StartHour"].ToString();
            if (!string.IsNullOrWhiteSpace(raw))
            {
                if (decimal.TryParse(raw, NumberStyles.Number, CultureInfo.CurrentCulture, out var val) ||
                    decimal.TryParse(raw.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out val) ||
                    decimal.TryParse(raw.Replace('.', ','), NumberStyles.Number, CultureInfo.GetCultureInfo("pl-PL"), out val))
                {
                    model.StartHour = val;
                    ModelState.Remove("StartHour");
                }
            }
        }

        if (string.IsNullOrEmpty(model?.EndHour.ToString()) && Request.HasFormContentType && Request.Form.ContainsKey("EndHour"))
        {
            var raw = Request.Form["EndHour"].ToString();
            if (!string.IsNullOrWhiteSpace(raw))
            {
                if (decimal.TryParse(raw, NumberStyles.Number, CultureInfo.CurrentCulture, out var val) ||
                    decimal.TryParse(raw.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out val) ||
                    decimal.TryParse(raw.Replace('.', ','), NumberStyles.Number, CultureInfo.GetCultureInfo("pl-PL"), out val))
                {
                    model.EndHour = val;
                    ModelState.Remove("EndHour");
                }
            }
        }

        if (!ModelState.IsValid)
        {
            model.Employees = await GetEmployeeItemsAsync();
            model.TypeOptions = GetTypeOptions();
            return View(model);
        }

        if (model.EmployeeId == null || model.Date == null)
        {
            ModelState.AddModelError(string.Empty, "Employee and date are required.");
            model.Employees = await GetEmployeeItemsAsync();
            model.TypeOptions = GetTypeOptions();
            return View(model);
        }

        // note: we detect 24-hour violations but do not block saving here; view will indicate violations

        var entry = new ScheduleEntry
        {
            EmployeeId = model.EmployeeId.Value,
            Date = model.Date.Value.Date,
            Type = model.Type,
            StartHour = model.StartHour,
            EndHour = model.EndHour,
            Notes = model.Notes
        };

        _context.ScheduleEntries.Add(entry);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new { week = entry.Date.ToString("yyyy-MM-dd") });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var entry = await _context.ScheduleEntries.FindAsync(id);
        if (entry == null)
        {
            return NotFound();
        }

        var model = new ScheduleEntryEditViewModel
        {
            Id = entry.Id,
            EmployeeId = entry.EmployeeId,
            Date = entry.Date,
            Type = entry.Type,
            StartHour = entry.StartHour,
            EndHour = entry.EndHour,
            Notes = entry.Notes,
            Employees = await GetEmployeeItemsAsync(),
            TypeOptions = GetTypeOptions()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ScheduleEntryEditViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        // Parse StartHour/EndHour similar to Create to handle locale decimal separators
        if (Request.HasFormContentType && Request.Form.ContainsKey("StartHour") && (model.StartHour == null))
        {
            var raw = Request.Form["StartHour"].ToString();
            if (!string.IsNullOrWhiteSpace(raw))
            {
                if (decimal.TryParse(raw, NumberStyles.Number, CultureInfo.CurrentCulture, out var val) ||
                    decimal.TryParse(raw.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out val) ||
                    decimal.TryParse(raw.Replace('.', ','), NumberStyles.Number, CultureInfo.GetCultureInfo("pl-PL"), out val))
                {
                    model.StartHour = val;
                    ModelState.Remove("StartHour");
                }
            }
        }

        if (Request.HasFormContentType && Request.Form.ContainsKey("EndHour") && (model.EndHour == null))
        {
            var raw = Request.Form["EndHour"].ToString();
            if (!string.IsNullOrWhiteSpace(raw))
            {
                if (decimal.TryParse(raw, NumberStyles.Number, CultureInfo.CurrentCulture, out var val) ||
                    decimal.TryParse(raw.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out val) ||
                    decimal.TryParse(raw.Replace('.', ','), NumberStyles.Number, CultureInfo.GetCultureInfo("pl-PL"), out val))
                {
                    model.EndHour = val;
                    ModelState.Remove("EndHour");
                }
            }
        }

        if (!ModelState.IsValid || model.EmployeeId == null || model.Date == null)
        {
            model.Employees = await GetEmployeeItemsAsync();
            model.TypeOptions = GetTypeOptions();
            return View(model);
        }

        var entry = await _context.ScheduleEntries.FindAsync(id);
        if (entry == null)
        {
            return NotFound();
        }

        // note: we detect 24-hour violations but do not block saving on edit either

        entry.EmployeeId = model.EmployeeId.Value;
        entry.Date = model.Date.Value.Date;
        entry.Type = model.Type;
        entry.StartHour = model.StartHour;
        entry.EndHour = model.EndHour;
        entry.Notes = model.Notes;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { week = entry.Date.ToString("yyyy-MM-dd") });
    }

    public async Task<IActionResult> Delete(int id)
    {
        var entry = await _context.ScheduleEntries
            .Include(item => item.Employee)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (entry == null)
        {
            return NotFound();
        }

        return View(entry);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var entry = await _context.ScheduleEntries.FindAsync(id);
        if (entry != null)
        {
            _context.ScheduleEntries.Remove(entry);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private static DateTime GetWeekStart(DateTime date)
    {
        var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.Date.AddDays(-diff);
    }

    private static string NormalizeViewMode(string? view)
    {
        return view?.Trim().ToLowerInvariant() switch
        {
            "employee" => "employee",
            "month" => "month",
            "day" => "day",
            _ => "week"
        };
    }

    private static DateTime GetViewStart(DateTime date, string viewMode)
    {
        return viewMode switch
        {
            "day" => date.Date,
            "month" or "employee" => new DateTime(date.Year, date.Month, 1),
            _ => GetWeekStart(date)
        };
    }

    private static IEnumerable<DateTime> GetVisibleDays(DateTime startDate, string viewMode)
    {
        return viewMode switch
        {
            "day" => new[] { startDate.Date },
            "month" or "employee" => Enumerable.Range(0, DateTime.DaysInMonth(startDate.Year, startDate.Month))
                .Select(offset => startDate.Date.AddDays(offset)),
            _ => Enumerable.Range(0, 7).Select(offset => startDate.Date.AddDays(offset))
        };
    }

    private async Task<IEnumerable<SelectListItem>> GetEmployeeItemsAsync()
    {
        var employees = await _context.Employees
            .OrderBy(employee => employee.LastName)
            .ThenBy(employee => employee.FirstName)
            .ToListAsync();

        return new[]
        {
            new SelectListItem
            {
                Value = string.Empty,
                Text = "-- Select employee --"
            }
        }.Concat(employees.Select(employee => new SelectListItem
        {
            Value = employee.Id.ToString(),
            Text = employee.FullName
        }));
    }

    private static IEnumerable<SelectListItem> GetTypeOptions()
    {
        return EntryTypes.Select(type => new SelectListItem
        {
            Value = type,
            Text = type
        });
    }

    private static bool IsHoliday(DateTime date)
    {
        var fixedHolidays = new[] { "1-1", "6-1", "5-4", "6-4", "1-5", "3-5", "24-5", "4-6", "15-8", "1-11", "11-11", "24-12", "25-12", "26-12" };
        var sundayExceptions = new[] { "25-1", "29-3", "26-4", "28-6", "30-8", "6-12", "13-12", "20-12" };
        var key = $"{date.Day}-{date.Month}";
        if (fixedHolidays.Contains(key)) return true;
        if (date.DayOfWeek == DayOfWeek.Sunday && !sundayExceptions.Contains(key)) return true;
        return false;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateBatch([FromForm] int employeeId, [FromForm] DateTime startDate, [FromForm] DateTime endDate, [FromForm] string type, [FromForm] decimal? startHour, [FromForm] decimal? endHour, [FromForm] string notes, [FromForm] bool ignoreHolidays = true)
    {
        var employee = await _context.Employees.FindAsync(employeeId);

        if (startDate > endDate)
        {
            ModelState.AddModelError(string.Empty, "Start date must be before end date.");
            return RedirectToAction(nameof(Index), new { week = startDate.ToString("yyyy-MM-dd") });
        }

        var created = 0;
        var addedEntries = new List<ScheduleEntry>();
        for (var d = startDate.Date; d <= endDate.Date; d = d.AddDays(1))
        {
            if (ignoreHolidays && IsHoliday(d)) continue;

            // skip if entry already exists for that employee/date
            var exists = await _context.ScheduleEntries.AnyAsync(e => e.EmployeeId == employeeId && e.Date == d);
            if (exists) continue;

            // validation applies only to employees without contract zlecenie
            if (startHour.HasValue && employee != null && !employee.IsContractZlecenie)
            {
                // check previous in DB
                var prev = await _context.ScheduleEntries
                    .Where(e => e.EmployeeId == employeeId && e.Date < d && (e.Type == "Normal" || e.Type == "Noc") && e.StartHour.HasValue)
                    .OrderByDescending(e => e.Date)
                    .FirstOrDefaultAsync();

                DateTime? prevStart = null;
                if (prev != null)
                {
                    prevStart = prev.Date.Date.AddHours((double)prev.StartHour.Value);
                }

                // check previous added in this batch
                var prevAdded = addedEntries.Where(e => e.EmployeeId == employeeId && e.Date < d && e.StartHour.HasValue).OrderByDescending(e => e.Date).FirstOrDefault();
                if (prevAdded != null)
                {
                    var addedStart = prevAdded.Date.Date.AddHours((double)prevAdded.StartHour.Value);
                    if (prevStart == null || addedStart > prevStart) prevStart = addedStart;
                }

                if (prevStart.HasValue)
                {
                    var currStart = d.Date.AddHours((double)startHour.Value);
                    if ((currStart - prevStart.Value) < TimeSpan.FromHours(24))
                    {
                        TempData["Error"] = "Nie można zaplanować rozpoczęcia dnia pracy przed upływem 24 godzin od poprzedniego początku pracy. Operacja przerwana.";
                        return RedirectToAction(nameof(Index), new { week = startDate.ToString("yyyy-MM-dd") });
                    }
                }
            }

            var entry = new ScheduleEntry
            {
                EmployeeId = employeeId,
                Date = d,
                Type = type,
                StartHour = startHour,
                EndHour = endHour,
                Notes = notes
            };

            _context.ScheduleEntries.Add(entry);
            addedEntries.Add(entry);
            created++;
        }

        if (created > 0)
        {
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index), new { week = startDate.ToString("yyyy-MM-dd"), view = "month" });
    }
}
