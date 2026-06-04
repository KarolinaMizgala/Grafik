using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleAppCore.Models;

public class Employee
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Imię")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [Display(Name = "Nazwisko")]
    public string LastName { get; set; } = string.Empty;

    [Display(Name = "Umowa zlecenie")]
    public bool IsContractZlecenie { get; set; }

    [Range(typeof(int), "1", "120")]
    [Display(Name = "Ilość miesięcy")]
    public int ContractMonths { get; set; } = 1;

    [Range(typeof(decimal), "0", "1000")]
    [Display(Name = "Norma godzin")]
    public decimal NormHours { get; set; } = 8;

    [Range(typeof(decimal), "0", "365")]
    [Display(Name = "Norma dni")]
    public decimal NormDays { get; set; } = 5;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedDate { get; set; }

    public ICollection<ScheduleEntry> ScheduleEntries { get; set; } = new List<ScheduleEntry>();

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}".Trim();
}
