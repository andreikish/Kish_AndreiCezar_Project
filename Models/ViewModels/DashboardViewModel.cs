using Kish_AndreiCezar_Project.Models;

namespace Kish_AndreiCezar_Project.Models.ViewModels;

public class DashboardViewModel
{
    public int ManufacturersCount { get; set; }
    public int CarModelsCount { get; set; }
    public int CustomersCount { get; set; }
    public int MechanicsCount { get; set; }
    public int TicketsCount { get; set; }

    public List<ServiceTicket> LatestTickets { get; set; } = new();

    public PredictionFormModel PredictionForm { get; set; } = new();
    public bool? NeedsMaintenance { get; set; }
    public string? MaintenanceStatus { get; set; }

    public GrpcFormModel GrpcForm { get; set; } = new();
    public GrpcResultModel? GrpcResult { get; set; }
}

public class PredictionFormModel
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

public class PredictionResult
{
    public bool NeedsMaintenance { get; set; }
    public string MaintenanceStatus { get; set; } = string.Empty;
}

public class GrpcFormModel
{
    public string CarModel { get; set; } = string.Empty;
    public int MileageKm { get; set; }
}

public class GrpcResultModel
{
    public string Recommendation { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public List<string> SuggestedTasks { get; set; } = new();
}

