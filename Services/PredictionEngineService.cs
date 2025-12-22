using Microsoft.ML;
using Microsoft.ML.Data;

namespace Kish_AndreiCezar_Project.Services;

public class PredictionEngineService
{
    private readonly MLContext _mlContext = new();
    private readonly PredictionEngine<ServiceData, ServicePrediction> _predictionEngine;

    public PredictionEngineService()
    {
        var samples = new List<ServiceData>
        {
            new() { MileageKm = 40000f, EstimatedHours = 2, PremiumFlag = 0, Cost = 500 },
            new() { MileageKm = 80000f, EstimatedHours = 3.5f, PremiumFlag = 1, Cost = 1400 },
            new() { MileageKm = 120000f, EstimatedHours = 5, PremiumFlag = 1, Cost = 2200 },
            new() { MileageKm = 90000f, EstimatedHours = 2.5f, PremiumFlag = 0, Cost = 1100 },
            new() { MileageKm = 30000f, EstimatedHours = 1.5f, PremiumFlag = 0, Cost = 450 },
            new() { MileageKm = 150000f, EstimatedHours = 6, PremiumFlag = 1, Cost = 3200 },
            new() { MileageKm = 60000f, EstimatedHours = 2, PremiumFlag = 0, Cost = 750 }
        };

        var data = _mlContext.Data.LoadFromEnumerable(samples);

        var pipeline = _mlContext.Transforms.CopyColumns("Label", nameof(ServiceData.Cost))
            .Append(_mlContext.Transforms.Concatenate("Features", nameof(ServiceData.MileageKm), nameof(ServiceData.EstimatedHours), nameof(ServiceData.PremiumFlag)))
            .Append(_mlContext.Regression.Trainers.FastTree());

        var model = pipeline.Fit(data);
        _predictionEngine = _mlContext.Model.CreatePredictionEngine<ServiceData, ServicePrediction>(model);
    }

    public float PredictCost(int mileageKm, float estimatedHours, bool isPremiumBrand)
    {
        var result = _predictionEngine.Predict(new ServiceData
        {
            MileageKm = mileageKm,
            EstimatedHours = estimatedHours,
            PremiumFlag = isPremiumBrand ? 1f : 0f
        });

        var predicted = (float)Math.Round(result.Cost, 2);

        if (float.IsNaN(predicted) || predicted <= 0)
        {
            var baseRate = isPremiumBrand ? 220f : 180f;
            var mileageFactor = Math.Clamp(mileageKm / 10000f, 0f, 25f);
            predicted = (float)Math.Round(baseRate * estimatedHours + mileageFactor * 40f, 2);
        }

        return predicted;
    }

    private class ServiceData
    {
        public float MileageKm { get; set; }
        public float EstimatedHours { get; set; }
        public float PremiumFlag { get; set; }
        public float Cost { get; set; }
    }

    private class ServicePrediction
    {
        [ColumnName("Score")]
        public float Cost { get; set; }
    }
}

