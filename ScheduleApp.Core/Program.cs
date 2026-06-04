using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using ScheduleAppCore.Data;
using ScheduleAppCore.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ScheduleContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ScheduleContext>();
    Directory.CreateDirectory(Path.Combine(app.Environment.ContentRootPath, "App_Data"));
    db.Database.EnsureCreated();
    EnsureEmployeeColumns(db);
    // SampleDataSeeder.Seed(db);  // Wyłączone - baza danych będzie pusta
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static void EnsureEmployeeColumns(ScheduleContext db)
{
    var connection = db.Database.GetDbConnection();
    var shouldCloseConnection = connection.State != System.Data.ConnectionState.Open;

    if (shouldCloseConnection)
    {
        connection.Open();
    }

    try
    {
        var existingColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        using (var command = connection.CreateCommand())
        {
            command.CommandText = "PRAGMA table_info(Employees);";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                existingColumns.Add(reader.GetString(1));
            }
        }

        if (!existingColumns.Contains("IsContractZlecenie"))
        {
            db.Database.ExecuteSqlRaw("ALTER TABLE Employees ADD COLUMN IsContractZlecenie INTEGER NOT NULL DEFAULT 0;");
        }

        if (!existingColumns.Contains("ContractMonths"))
        {
            db.Database.ExecuteSqlRaw("ALTER TABLE Employees ADD COLUMN ContractMonths INTEGER NOT NULL DEFAULT 1;");
        }
    }
    finally
    {
        if (shouldCloseConnection)
        {
            connection.Close();
        }
    }
}
