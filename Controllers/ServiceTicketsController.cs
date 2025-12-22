using Kish_AndreiCezar_Project.Data;
using Kish_AndreiCezar_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Kish_AndreiCezar_Project.Controllers;

public class ServiceTicketsController : Controller
{
    private readonly AppDbContext _context;

    public ServiceTicketsController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(string? status, int? mechanicId, int? manufacturerId, string? sort)
    {
        var query = _context.ServiceTickets
            .Include(t => t.CarModel)!.ThenInclude(m => m!.Manufacturer)
            .Include(t => t.Customer)
            .Include(t => t.Mechanic)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(t => t.Status == status);

        if (mechanicId is not null)
            query = query.Where(t => t.MechanicId == mechanicId);

        if (manufacturerId is not null)
            query = query.Where(t => t.CarModel!.ManufacturerId == manufacturerId);

        query = sort switch
        {
            "date_desc" => query.OrderByDescending(t => t.IntakeDate),
            "cost" => query.OrderByDescending(t => t.EstimatedCost),
            "mileage" => query.OrderByDescending(t => t.MileageKm),
            _ => query.OrderBy(t => t.IntakeDate)
        };

        ViewBag.StatusOptions = new SelectList(_context.ServiceTickets.Select(t => t.Status).Distinct().ToList());
        ViewBag.Mechanics = new SelectList(_context.Mechanics.OrderBy(m => m.FullName), "Id", "FullName", mechanicId);
        ViewBag.Manufacturers = new SelectList(_context.Manufacturers.OrderBy(m => m.Name), "Id", "Name", manufacturerId);
        ViewBag.Sort = sort;

        return View(query.ToList());
    }

    public IActionResult Details(int id)
    {
        var ticket = _context.ServiceTickets
            .Include(t => t.CarModel)!.ThenInclude(m => m!.Manufacturer)
            .Include(t => t.Customer)
            .Include(t => t.Mechanic)
            .FirstOrDefault(t => t.Id == id);

        return ticket is null ? NotFound() : View(ticket);
    }

    [HttpGet]
    public IActionResult Create()
    {
        LoadSelects();
        return View(new ServiceTicket { IntakeDate = DateOnly.FromDateTime(DateTime.Today) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ServiceTicket ticket)
    {
        if (!ModelState.IsValid)
        {
            LoadSelects();
            return View(ticket);
        }

        _context.ServiceTickets.Add(ticket);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var ticket = _context.ServiceTickets.Find(id);
        if (ticket is null) return NotFound();
        LoadSelects(ticket);
        return View(ticket);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, ServiceTicket ticket)
    {
        if (id != ticket.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            LoadSelects(ticket);
            return View(ticket);
        }

        _context.Update(ticket);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var ticket = _context.ServiceTickets
            .Include(t => t.CarModel)!.ThenInclude(m => m!.Manufacturer)
            .Include(t => t.Customer)
            .Include(t => t.Mechanic)
            .FirstOrDefault(t => t.Id == id);
        return ticket is null ? NotFound() : View(ticket);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var ticket = _context.ServiceTickets.Find(id);
        if (ticket != null)
        {
            _context.ServiceTickets.Remove(ticket);
            _context.SaveChanges();
        }
        return RedirectToAction(nameof(Index));
    }

    private void LoadSelects(ServiceTicket? ticket = null)
    {
        ViewBag.CarModelId = new SelectList(_context.CarModels.Include(c => c.Manufacturer).OrderBy(c => c.Manufacturer!.Name).ThenBy(c => c.Name).ToList(), "Id", "Name", ticket?.CarModelId);
        ViewBag.CustomerId = new SelectList(_context.Customers.OrderBy(c => c.FullName), "Id", "FullName", ticket?.CustomerId);
        ViewBag.MechanicId = new SelectList(_context.Mechanics.OrderBy(m => m.FullName), "Id", "FullName", ticket?.MechanicId);
    }
}

