// Controllers/EmployeesController.cs
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class EmployeesController : Controller
{
    private readonly ApplicationDbContext _context;

    public EmployeesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string search, string sortOrder, int page = 1)
    {
        int pageSize = 5;
        var employees = _context.Employees.Include(e => e.Department).AsQueryable();

        if (!string.IsNullOrEmpty(search))
            employees = employees.Where(e => e.FirstName.Contains(search) || e.LastName.Contains(search));

        employees = sortOrder switch
        {
            "name" => employees.OrderBy(e => e.FirstName),
            "hireDate" => employees.OrderByDescending(e => e.HireDate),
            _ => employees
        };

        var pagedEmployees = await employees.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return View(pagedEmployees);
    }

    public IActionResult Create()
    {
        ViewBag.Departments = _context.Departments.ToList();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Employee employee)
    {
        if (ModelState.IsValid)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Departments = _context.Departments.ToList();
        return View(employee);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null) return NotFound();

        ViewBag.Departments = _context.Departments.ToList();
        return View(employee);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Employee employee)
    {
        if (ModelState.IsValid)
        {
            _context.Update(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Departments = _context.Departments.ToList();
        return View(employee);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null) return NotFound();
        return View(employee);
    }

    [HttpPost, ActionName("Delete")]
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
    