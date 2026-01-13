using Kish_AndreiCezar_Project.Data;
using Kish_AndreiCezar_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Kish_AndreiCezar_Project.Controllers;

public class CarModelsController : Controller
{
    private readonly AppDbContext _context;

    public CarModelsController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var models = _context.CarModels
            .Include(c => c.Manufacturer)
            .OrderBy(c => c.Manufacturer!.Name)
            .ThenBy(c => c.Name)
            .ToList();
        return View(models);
    }

    public IActionResult Details(int id)
    {
        var model = _context.CarModels.Include(c => c.Manufacturer).FirstOrDefault(c => c.Id == id);
        return model is null ? NotFound() : View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.ManufacturerId = new SelectList(_context.Manufacturers.OrderBy(m => m.Name), "Id", "Name");
        ViewBag.VehicleTypes = new SelectList(new[] { "Car", "Truck", "Motorcycle", "SUV", "Bus" });
        return View(new CarModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CarModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ManufacturerId = new SelectList(_context.Manufacturers, "Id", "Name", model.ManufacturerId);
            ViewBag.VehicleTypes = new SelectList(new[] { "Car", "Truck", "Motorcycle", "SUV", "Bus" }, model.VehicleType);
            return View(model);
        }

        _context.CarModels.Add(model);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var model = _context.CarModels.Find(id);
        if (model is null) return NotFound();

        ViewBag.ManufacturerId = new SelectList(_context.Manufacturers, "Id", "Name", model.ManufacturerId);
        ViewBag.VehicleTypes = new SelectList(new[] { "Car", "Truck", "Motorcycle", "SUV", "Bus" }, model.VehicleType);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, CarModel model)
    {
        if (id != model.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            ViewBag.ManufacturerId = new SelectList(_context.Manufacturers, "Id", "Name", model.ManufacturerId);
            ViewBag.VehicleTypes = new SelectList(new[] { "Car", "Truck", "Motorcycle", "SUV", "Bus" }, model.VehicleType);
            return View(model);
        }

        _context.Update(model);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var model = _context.CarModels.Include(c => c.Manufacturer).FirstOrDefault(c => c.Id == id);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var model = _context.CarModels.Find(id);
        if (model != null)
        {
            _context.CarModels.Remove(model);
            _context.SaveChanges();
        }
        return RedirectToAction(nameof(Index));
    }
}

