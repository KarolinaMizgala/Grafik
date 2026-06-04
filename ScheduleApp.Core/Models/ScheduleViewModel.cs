namespace ScheduleAppCore.Models;

public class ScheduleViewModel
{
    public DateTime WeekStart { get; set; }
    public string ViewMode { get; set; } = "week";
    public int? SelectedEmployeeId { get; set; }
    public List<Employee> Employees { get; set; } = new();
    public List<ScheduleEntry> Entries { get; set; } = new();

    public IEnumerable<DateTime> VisibleDays { get; set; } = Enumerable.Empty<DateTime>();
}
