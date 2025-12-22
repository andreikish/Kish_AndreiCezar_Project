using Kish_AndreiCezar_Project.Data;
using Kish_AndreiCezar_Project.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kish_AndreiCezar_Project.Controllers;

public class MechanicsController : Controller
{
    private readonly AppDbContext _context;

    public MechanicsController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var mechanics = _context.Mechanics.OrderBy(m => m.FullName).ToList();
        return View(mechanics);
    }

    public IActionResult Details(int id)
    {
        var mechanic = _context.Mechanics.Find(id);
        return mechanic is null ? NotFound() : View(mechanic);
    }

    [HttpGet]
    public IActionResult Create() => View(new Mechanic());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Mechanic mechanic)
    {
        if (!ModelState.IsValid) return View(mechanic);

        _context.Mechanics.Add(mechanic);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var mechanic = _context.Mechanics.Find(id);
        return mechanic is null ? NotFound() : View(mechanic);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Mechanic mechanic)
    {
        if (id != mechanic.Id) return BadRequest();

        if (!ModelState.IsValid) return View(mechanic);

        _context.Update(mechanic);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var mechanic = _context.Mechanics.Find(id);
        return mechanic is null ? NotFound() : View(mechanic);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var mechanic = _context.Mechanics.Find(id);
        if (mechanic != null)
        {
            _context.Mechanics.Remove(mechanic);
            _context.SaveChanges();
        }
        return RedirectToAction(nameof(Index));
    }
}

