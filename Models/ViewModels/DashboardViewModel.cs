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
    public float? PredictedCost { get; set; }

    public GrpcFormModel GrpcForm { get; set; } = new();
    public GrpcResultModel? GrpcResult { get; set; }
}

public class PredictionFormModel
{
    public int MileageKm { get; set; }
    public float EstimatedHours { get; set; }
    public bool IsPremiumBrand { get; set; }
}

public class PredictionResult
{
    public float PredictedCost { get; set; }
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

