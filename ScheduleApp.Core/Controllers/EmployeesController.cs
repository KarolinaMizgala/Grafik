using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScheduleAppCore.Data;
using ScheduleAppCore.Models;

namespace ScheduleAppCore.Controllers;

public class EmployeesController : Controller
{
    private readonly ScheduleContext _context;

    public EmployeesController(ScheduleContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var employees = await _context.Employees
            .OrderBy(employee => employee.LastName)
            .ThenBy(employee => employee.FirstName)
            .ToListAsync();

        return View(employees);
    }

    public IActionResult Create()
    {
        return View(new Employee());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Employee employee)
    {
        if (!ModelState.IsValid)
        {
            return View(employee);
        }

        employee.CreatedDate = DateTime.UtcNow;
        employee.NormDays = 5;
        _context.Add(employee);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        return View(employee);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Employee employee)
    {
        if (id != employee.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(employee);
        }

        var existingEmployee = await _context.Employees.FindAsync(id);
        if (existingEmployee == null)
        {
            return NotFound();
        }

        existingEmployee.FirstName = employee.FirstName;
        existingEmployee.LastName = employee.LastName;
        existingEmployee.IsContractZlecenie = employee.IsContractZlecenie;
        existingEmployee.ContractMonths = employee.ContractMonths;
        existingEmployee.NormHours = employee.NormHours;
        existingEmployee.ModifiedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _context.Employees
            .Include(item => item.ScheduleEntries)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (employee == null)
        {
            return NotFound();
        }

        return View(employee);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee != null)
        {
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
