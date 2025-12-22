using System.Diagnostics;
using System.Net.Http.Json;
using Grpc.Net.Client;
using Kish_AndreiCezar_Project.Data;
using Kish_AndreiCezar_Project.Models;
using Kish_AndreiCezar_Project.Models.ViewModels;
using Kish_AndreiCezar_Project.Protos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kish_AndreiCezar_Project.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<HomeController> _logger;

    public HomeController(AppDbContext context, IHttpClientFactory httpClientFactory, ILogger<HomeController> logger)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var vm = await BuildDashboardAsync();
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Predict(DashboardViewModel model)
    {
        var vm = await BuildDashboardAsync();
        vm.PredictionForm = model.PredictionForm;

        try
        {
            var client = _httpClientFactory.CreateClient();
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var response = await client.PostAsJsonAsync($"{baseUrl}/api/predict", vm.PredictionForm);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PredictionResult>();
                vm.PredictedCost = result?.PredictedCost;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Eroare la consumarea serviciului REST ML");
            ModelState.AddModelError("", "Nu s-a putut apela serviciul de predictie.");
        }

        return View(nameof(Index), vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Advisor(DashboardViewModel model)
    {
        var vm = await BuildDashboardAsync();
        vm.GrpcForm = model.GrpcForm;

        try
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            using var channel = GrpcChannel.ForAddress(baseUrl);
            var client = new MaintenanceAdvisor.MaintenanceAdvisorClient(channel);
            var reply = await client.GetScheduleAsync(new MaintenanceRequest
            {
                CarModel = model.GrpcForm.CarModel,
                MileageKm = model.GrpcForm.MileageKm
            });

            vm.GrpcResult = new GrpcResultModel
            {
                Recommendation = reply.Recommendation,
                Severity = reply.Severity,
                SuggestedTasks = reply.SuggestedTasks.ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Eroare la apelul serviciului gRPC");
            ModelState.AddModelError("", "Nu s-a putut apela serviciul gRPC.");
        }

        return View(nameof(Index), vm);
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private async Task<DashboardViewModel> BuildDashboardAsync()
    {
        var latestTickets = await _context.ServiceTickets
            .Include(t => t.CarModel)!.ThenInclude(cm => cm!.Manufacturer)
            .Include(t => t.Customer)
            .Include(t => t.Mechanic)
            .OrderByDescending(t => t.IntakeDate)
            .Take(5)
            .ToListAsync();

        return new DashboardViewModel
        {
            ManufacturersCount = await _context.Manufacturers.CountAsync(),
            CarModelsCount = await _context.CarModels.CountAsync(),
            CustomersCount = await _context.Customers.CountAsync(),
            MechanicsCount = await _context.Mechanics.CountAsync(),
            TicketsCount = await _context.ServiceTickets.CountAsync(),
            LatestTickets = latestTickets,
            PredictionForm = new PredictionFormModel { MileageKm = 80000, EstimatedHours = 3, IsPremiumBrand = false },
            GrpcForm = new GrpcFormModel { CarModel = "Duster", MileageKm = 90000 }
        };
    }
}
