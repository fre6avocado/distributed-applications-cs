using EmployeeManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class RolesController : Controller
{
    private readonly ApplicationDbContext _context;

    public RolesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult ManageRoles()
    {
        var users = _context.Users.ToList();
        return View(users);
    }

    [HttpPost]
    public IActionResult UpdateRole(int userId, UserRole role)
    {
        var user = _context.Users.Find(userId);
        if (user == null) return NotFound();

        user.Role = role;
        _context.SaveChanges();

        return RedirectToAction("ManageRoles");
    }
}
