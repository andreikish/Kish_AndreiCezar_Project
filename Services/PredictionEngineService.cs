using Kish_AndreiCezar_Project;

namespace Kish_AndreiCezar_Project.Services;

public class PredictionEngineService
{
    public bool PredictMaintenanceNeed(MaintenancePredictionInput input)
    {
        var modelInput = new MaintenancePredict.ModelInput
        {
            Vehicle_Model = input.VehicleModel ?? "Car",
            Mileage = input.Mileage,
            Maintenance_History = input.MaintenanceHistory ?? "Good",
            Reported_Issues = input.ReportedIssues,
            Vehicle_Age = input.VehicleAge,
            Fuel_Type = input.FuelType ?? "Petrol",
            Transmission_Type = input.TransmissionType ?? "Manual",
            Engine_Size = input.EngineSize,
            Odometer_Reading = input.OdometerReading,
            Last_Service_Date = input.LastServiceDate ?? DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd"),
            Warranty_Expiry_Date = input.WarrantyExpiryDate ?? DateTime.Now.AddYears(2).ToString("yyyy-MM-dd"),
            Owner_Type = input.OwnerType ?? "First",
            Insurance_Premium = input.InsurancePremium,
            Service_History = input.ServiceHistory,
            Accident_History = input.AccidentHistory,
            Fuel_Efficiency = input.FuelEfficiency,
            Tire_Condition = input.TireCondition ?? "Good",
            Brake_Condition = input.BrakeCondition ?? "Good",
            Battery_Status = input.BatteryStatus ?? "Good"
        };

        var result = MaintenancePredict.Predict(modelInput);
        
        return result.Score >= 0.5f;
    }
}

public class MaintenancePredictionInput
{
    public string? VehicleModel { get; set; }
    public float Mileage { get; set; }
    public string? MaintenanceHistory { get; set; }
    public float ReportedIssues { get; set; }
    public float VehicleAge { get; set; }
    public string? FuelType { get; set; }
    public string? TransmissionType { get; set; }
    public float EngineSize { get; set; }
    public float OdometerReading { get; set; }
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

