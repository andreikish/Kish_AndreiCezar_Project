using Kish_AndreiCezar_Project.Data;
using Kish_AndreiCezar_Project.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kish_AndreiCezar_Project.Controllers;

public class CustomersController : Controller
{
    private readonly AppDbContext _context;

    public CustomersController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var customers = _context.Customers.OrderBy(c => c.FullName).ToList();
        return View(customers);
    }

    public IActionResult Details(int id)
    {
        var customer = _context.Customers.Find(id);
        return customer is null ? NotFound() : View(customer);
    }

    [HttpGet]
    public IActionResult Create() => View(new Customer());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Customer customer)
    {
        if (!ModelState.IsValid) return View(customer);

        _context.Customers.Add(customer);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var customer = _context.Customers.Find(id);
        return customer is null ? NotFound() : View(customer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Customer customer)
    {
        if (id != customer.Id) return BadRequest();

        if (!ModelState.IsValid) return View(customer);

        _context.Update(customer);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var customer = _context.Customers.Find(id);
        return customer is null ? NotFound() : View(customer);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var customer = _context.Customers.Find(id);
        if (customer != null)
        {
            _context.Customers.Remove(customer);
            _context.SaveChanges();
        }
        return RedirectToAction(nameof(Index));
    }
}

