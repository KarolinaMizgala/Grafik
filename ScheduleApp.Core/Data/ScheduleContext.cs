using Microsoft.EntityFrameworkCore;
using ScheduleAppCore.Models;

namespace ScheduleAppCore.Data;

public class ScheduleContext : DbContext
{
    public ScheduleContext(DbContextOptions<ScheduleContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<ScheduleEntry> ScheduleEntries => Set<ScheduleEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.NormHours).HasPrecision(5, 2);
            entity.Property(e => e.NormDays).HasPrecision(5, 2);

            entity.HasMany(e => e.ScheduleEntries)
                .WithOne(e => e.Employee)
                .HasForeignKey(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ScheduleEntry>(entity =>
        {
            entity.Property(e => e.Type).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.Date).HasColumnType("date");
        });
    }
}
