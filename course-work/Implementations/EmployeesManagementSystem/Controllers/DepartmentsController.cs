using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

[Authorize] // Защита на целия контролер
public class DepartmentsController : Controller
{
    private readonly ApplicationDbContext _context;

    public DepartmentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 1. Показване на списък с отдели (Read)
    public async Task<IActionResult> Index(string search, string sortOrder, int page = 1)
    {
        int pageSize = 5; // Брой записи на страница
        var departments = _context.Departments.AsQueryable();

        // Търсене
        if (!string.IsNullOrEmpty(search))
        {
            departments = departments.Where(d => d.Name.Contains(search) || d.Location.Contains(search));
        }

        // Сортиране
        departments = sortOrder switch
        {
            "name" => departments.OrderBy(d => d.Name),
            "location" => departments.OrderBy(d => d.Location),
            _ => departments
        };

        // Странициране
        var pagedDepartments = await departments.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return View(pagedDepartments);
    }

    // 2. Показване на детайли за отдел (Read)
    public async Task<IActionResult> Details(int id)
    {
        var department = await _context.Departments
            .Include(d => d.Employees) // Включване на свързани данни (служители)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (department == null)
        {
            return NotFound();
        }

        return View(department);
    }

    // 3. Показване на форма за създаване на нов отдел (Create)
    [Authorize(Roles = "Admin")] // Само администратори могат да създават отдели
    public IActionResult Create()
    {
        return View();
    }

    // 4. Създаване на нов отдел (Create)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Department department)
    {
        if (ModelState.IsValid)
        {
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(department);
    }

    // 5. Показване на форма за редактиране на отдел (Update)
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null)
        {
            return NotFound();
        }
        return View(department);
    }

    // 6. Редактиране на съществуващ отдел (Update)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Department department)
    {
        if (id != department.Id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(department);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Departments.Any(d => d.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(department);
    }

    // 7. Показване на потвърждение за изтриване на отдел (Delete)
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null)
        {
            return NotFound();
        }
        return View(department);
    }

    // 8. Изтриване на отдел (Delete)
    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department != null)
        {
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
