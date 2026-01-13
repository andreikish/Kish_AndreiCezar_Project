using System.Diagnostics;
using System.Net.Http.Json;
using Grpc.Net.Client;
using System.Net.Http;
using Kish_AndreiCezar_Project.Data;
using Kish_AndreiCezar_Project.Models;
using Kish_AndreiCezar_Project.Models.ViewModels;
using Kish_AndreiCezar_Project.Protos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Kish_AndreiCezar_Project.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;

    public HomeController(AppDbContext context, IHttpClientFactory httpClientFactory, ILogger<HomeController> logger, IConfiguration configuration)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _configuration = configuration;
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
        ModelState.Remove("GrpcForm.CarModelId");
        ModelState.Remove("GrpcForm.MileageKm");
        ModelState.Remove("GrpcForm");
        
        var vm = await BuildDashboardAsync();
        vm.PredictionForm = model.PredictionForm;
        vm.GrpcForm = new GrpcFormModel { MileageKm = 145000 };

        try
        {
            var mlServiceUrl = _configuration["MLService:BaseUrl"] ?? "http://localhost:5002";
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync($"{mlServiceUrl}/api/predict", vm.PredictionForm);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PredictionResult>();
                vm.NeedsMaintenance = result?.NeedsMaintenance;
                vm.MaintenanceStatus = result?.MaintenanceStatus;
            }
            else
            {
                _logger.LogError("ML Service returned error: {StatusCode} - {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
                ModelState.AddModelError("", "Serviciul ML a returnat o eroare.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Eroare la consumarea serviciului REST ML");
            ModelState.AddModelError("", $"Nu s-a putut apela serviciul de predictie: {ex.Message}");
        }

        return View(nameof(Index), vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Advisor(DashboardViewModel model)
    {
        var vm = await BuildDashboardAsync();
        vm.GrpcForm = model.GrpcForm;

        if (!model.GrpcForm.CarModelId.HasValue)
        {
            ModelState.AddModelError("GrpcForm.CarModelId", "Selectează un model de vehicul.");
            return View(nameof(Index), vm);
        }

        var localCarModel = await _context.CarModels
            .Include(c => c.Manufacturer)
            .FirstOrDefaultAsync(c => c.Id == model.GrpcForm.CarModelId.Value);

        if (localCarModel == null)
        {
            ModelState.AddModelError("GrpcForm.CarModelId", "Modelul selectat nu a fost găsit.");
            return View(nameof(Index), vm);
        }

        var vehicleType = localCarModel.VehicleType;

        try
        {
            var grpcServerUrl = _configuration["GrpcServer:BaseUrl"] ?? "http://localhost:5001";
            
            var httpClientHandler = new HttpClientHandler();
            var httpClient = new HttpClient(httpClientHandler)
            {
                DefaultRequestVersion = new Version(2, 0),
                DefaultVersionPolicy = System.Net.Http.HttpVersionPolicy.RequestVersionOrHigher
            };
            
            var channelOptions = new GrpcChannelOptions
            {
                HttpClient = httpClient
            };
            
            using var channel = GrpcChannel.ForAddress(grpcServerUrl, channelOptions);
            var client = new MaintenanceAdvisor.MaintenanceAdvisorClient(channel);
            
            var reply = await client.GetScheduleAsync(new MaintenanceRequest
            {
                CarModel = vehicleType,
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
            _logger.LogError(ex, "Eroare la apelul serviciului gRPC: {Message}", ex.Message);
            ModelState.AddModelError("", $"Nu s-a putut apela serviciul gRPC: {ex.Message}");
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

        var carModels = await _context.CarModels
            .Include(c => c.Manufacturer)
            .OrderBy(c => c.Manufacturer!.Name)
            .ThenBy(c => c.Name)
            .ToListAsync();

        var carModelSelectList = carModels.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = $"{c.Manufacturer?.Name} {c.Name} ({c.VehicleType})"
        }).ToList();

        ViewBag.CarModelId = new SelectList(carModelSelectList, "Value", "Text");

        return new DashboardViewModel
        {
            ManufacturersCount = await _context.Manufacturers.CountAsync(),
            CarModelsCount = await _context.CarModels.CountAsync(),
            CustomersCount = await _context.Customers.CountAsync(),
            MechanicsCount = await _context.Mechanics.CountAsync(),
            TicketsCount = await _context.ServiceTickets.CountAsync(),
            LatestTickets = latestTickets,
            PredictionForm = new PredictionFormModel 
            { 
                VehicleModel = "Car",
                MileageKm = 72000,
                VehicleAge = 4,
                MaintenanceHistory = "Good",
                ReportedIssues = 3,
                FuelType = "Petrol",
                TransmissionType = "Automatic",
                EngineSize = 1000,
                LastServiceDate = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd"),
                WarrantyExpiryDate = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd"),
                OwnerType = "First",
                InsurancePremium = 24050,
                ServiceHistory = 6,
                AccidentHistory = 1,
                FuelEfficiency = 16.39f,
                TireCondition = "Good",
                BrakeCondition = "New",
                BatteryStatus = "Good"
            },
            GrpcForm = new GrpcFormModel { MileageKm = 145000 }
        };
    }
}
