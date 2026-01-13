using Kish_AndreiCezar_Project.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kish_AndreiCezar_Project.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class PredictController : ControllerBase
{
    private readonly PredictionEngineService _prediction;

    public PredictController(PredictionEngineService prediction)
    {
        _prediction = prediction;
    }

    [HttpPost]
    public ActionResult<PredictionResponse> Post([FromBody] PredictionRequest request)
    {
        var input = new MaintenancePredictionInput
        {
            VehicleModel = request.VehicleModel,
            Mileage = request.MileageKm,
            MaintenanceHistory = request.MaintenanceHistory,
            ReportedIssues = request.ReportedIssues,
            VehicleAge = request.VehicleAge,
            FuelType = request.FuelType,
            TransmissionType = request.TransmissionType,
            EngineSize = request.EngineSize,
            OdometerReading = request.MileageKm,
            LastServiceDate = request.LastServiceDate,
            WarrantyExpiryDate = request.WarrantyExpiryDate,
            OwnerType = request.OwnerType,
            InsurancePremium = request.InsurancePremium,
            ServiceHistory = request.ServiceHistory,
            AccidentHistory = request.AccidentHistory,
            FuelEfficiency = request.FuelEfficiency,
            TireCondition = request.TireCondition,
            BrakeCondition = request.BrakeCondition,
            BatteryStatus = request.BatteryStatus
        };

        var needsMaintenance = _prediction.PredictMaintenanceNeed(input);
        return Ok(new PredictionResponse 
        { 
            NeedsMaintenance = needsMaintenance,
            MaintenanceStatus = needsMaintenance ? "Da, este nevoie de maintenance" : "Nu, nu este nevoie de maintenance"
        });
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

