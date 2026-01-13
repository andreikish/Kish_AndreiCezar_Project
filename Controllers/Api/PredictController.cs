using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace Kish_AndreiCezar_Project.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class PredictController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PredictController> _logger;

    public PredictController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<PredictController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<PredictionResponse>> Post([FromBody] PredictionRequest request)
    {
        try
        {
            var mlServiceUrl = _configuration["MLService:BaseUrl"] ?? "http://localhost:5002";
            var httpClient = _httpClientFactory.CreateClient();
            
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await httpClient.PostAsync($"{mlServiceUrl}/api/predict", content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var predictionResponse = JsonSerializer.Deserialize<PredictionResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                return Ok(predictionResponse);
            }
            else
            {
                _logger.LogError("ML Service returned error: {StatusCode} - {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
                return StatusCode(500, new PredictionResponse 
                { 
                    NeedsMaintenance = false,
                    MaintenanceStatus = "Eroare la comunicarea cu serviciul ML"
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Eroare la apelarea serviciului ML");
            return StatusCode(500, new PredictionResponse 
            { 
                NeedsMaintenance = false,
                MaintenanceStatus = $"Eroare: {ex.Message}"
            });
        }
    }
}

public class PredictionRequest
{
    public string? VehicleModel { get; set; }
    public float MileageKm { get; set; }
    public string? MaintenanceHistory { get; set; }
    public float ReportedIssues { get; set; }
    public float VehicleAge { get; set; }
    public string? FuelType { get; set; }
    public string? TransmissionType { get; set; }
    public float EngineSize { get; set; }
    public string? LastServiceDate { get; set; }
    public string? WarrantyExpiryDate { get; set; }
    public string? OwnerType { get; set; }
    public float InsurancePremium { get; set; }
    public float ServiceHistory { get; set; }
    public float AccidentHistory { get; set; }
    public float FuelEfficiency { get; set; }
    public string? TireCondition { get; set; }
    public string? BrakeCondition { get; set; }
    public string? BatteryStatus { get; set; }
}

public class PredictionResponse
{
    public bool NeedsMaintenance { get; set; }
    public string MaintenanceStatus { get; set; } = string.Empty;
}

