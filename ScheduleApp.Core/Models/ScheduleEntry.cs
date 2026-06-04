using System.ComponentModel.DataAnnotations;

namespace ScheduleAppCore.Models;

public class ScheduleEntry
{
    public int Id { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    public Employee? Employee { get; set; }

    [DataType(DataType.Date)]
    public DateTime Date { get; set; }

    [Required]
    [StringLength(50)]
    public string Type { get; set; } = "Normal";

    [Range(typeof(decimal), "0", "24")]
    public decimal? StartHour { get; set; }

    [Range(typeof(decimal), "0", "24")]
    public decimal? EndHour { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}
