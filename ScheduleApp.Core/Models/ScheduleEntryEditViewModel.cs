using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ScheduleAppCore.Models;

public class ScheduleEntryEditViewModel
{
    public int Id { get; set; }

    [Required]
    public int? EmployeeId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime? Date { get; set; }

    [Required]
    [StringLength(50)]
    public string Type { get; set; } = "Normal";

    [Range(typeof(decimal), "0", "24")]
    public decimal? StartHour { get; set; }

    [Range(typeof(decimal), "0", "24")]
    public decimal? EndHour { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    public IEnumerable<SelectListItem> Employees { get; set; } = Enumerable.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> TypeOptions { get; set; } = Enumerable.Empty<SelectListItem>();
}
