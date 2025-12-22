using Kish_AndreiCezar_Project.Data;
using Kish_AndreiCezar_Project.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kish_AndreiCezar_Project.Controllers;

public class ManufacturersController : Controller
{
    private readonly AppDbContext _context;

    public ManufacturersController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var data = _context.Manufacturers.OrderBy(m => m.Name).ToList();
        return View(data);
    }

    public IActionResult Details(int id)
    {
        var manufacturer = _context.Manufacturers
            .Where(m => m.Id == id)
            .Select(m => new Manufacturer
            {
                Id = m.Id,
                Name = m.Name,
                Country = m.Country,
                Models = m.Models.OrderBy(cm => cm.Name).ToList()
            }).FirstOrDefault();

        if (manufacturer is null)
            return NotFound();

        return View(manufacturer);
    }

    [HttpGet]
    public IActionResult Create() => View(new Manufacturer());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Manufacturer manufacturer)
    {
        if (!ModelState.IsValid)
            return View(manufacturer);

        _context.Manufacturers.Add(manufacturer);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var manufacturer = _context.Manufacturers.Find(id);
        return manufacturer is null ? NotFound() : View(manufacturer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Manufacturer manufacturer)
    {
        if (id != manufacturer.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(manufacturer);

        _context.Update(manufacturer);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var manufacturer = _context.Manufacturers.Find(id);
        return manufacturer is null ? NotFound() : View(manufacturer);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var manufacturer = _context.Manufacturers.Find(id);
        if (manufacturer != null)
        {
            _context.Manufacturers.Remove(manufacturer);
            _context.SaveChanges();
        }
        return RedirectToAction(nameof(Index));
    }
}

