using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
namespace Kish_AndreiCezar_Project
{
    public partial class MaintenancePredict
    {
        #region model input class
        public class ModelInput
        {
            [LoadColumn(0)]
            [ColumnName(@"Vehicle_Model")]
            public string Vehicle_Model { get; set; }

            [LoadColumn(1)]
            [ColumnName(@"Mileage")]
            public float Mileage { get; set; }

            [LoadColumn(2)]
            [ColumnName(@"Maintenance_History")]
            public string Maintenance_History { get; set; }

            [LoadColumn(3)]
            [ColumnName(@"Reported_Issues")]
            public float Reported_Issues { get; set; }

            [LoadColumn(4)]
            [ColumnName(@"Vehicle_Age")]
            public float Vehicle_Age { get; set; }

            [LoadColumn(5)]
            [ColumnName(@"Fuel_Type")]
            public string Fuel_Type { get; set; }

            [LoadColumn(6)]
            [ColumnName(@"Transmission_Type")]
            public string Transmission_Type { get; set; }

            [LoadColumn(7)]
            [ColumnName(@"Engine_Size")]
            public float Engine_Size { get; set; }

            [LoadColumn(8)]
            [ColumnName(@"Odometer_Reading")]
            public float Odometer_Reading { get; set; }

            [LoadColumn(9)]
            [ColumnName(@"Last_Service_Date")]
            public string Last_Service_Date { get; set; }

            [LoadColumn(10)]
            [ColumnName(@"Warranty_Expiry_Date")]
            public string Warranty_Expiry_Date { get; set; }

            [LoadColumn(11)]
            [ColumnName(@"Owner_Type")]
            public string Owner_Type { get; set; }

            [LoadColumn(12)]
            [ColumnName(@"Insurance_Premium")]
            public float Insurance_Premium { get; set; }

            [LoadColumn(13)]
            [ColumnName(@"Service_History")]
            public float Service_History { get; set; }

            [LoadColumn(14)]
            [ColumnName(@"Accident_History")]
            public float Accident_History { get; set; }

            [LoadColumn(15)]
            [ColumnName(@"Fuel_Efficiency")]
            public float Fuel_Efficiency { get; set; }

            [LoadColumn(16)]
            [ColumnName(@"Tire_Condition")]
            public string Tire_Condition { get; set; }

            [LoadColumn(17)]
            [ColumnName(@"Brake_Condition")]
            public string Brake_Condition { get; set; }

            [LoadColumn(18)]
            [ColumnName(@"Battery_Status")]
            public string Battery_Status { get; set; }

            [LoadColumn(19)]
            [ColumnName(@"Need_Maintenance")]
            public float Need_Maintenance { get; set; }

        }

        #endregion

        #region model output class
        public class ModelOutput
        {
            [ColumnName(@"Vehicle_Model")]
            public float[] Vehicle_Model { get; set; }

            [ColumnName(@"Mileage")]
            public float Mileage { get; set; }

            [ColumnName(@"Maintenance_History")]
            public float[] Maintenance_History { get; set; }

            [ColumnName(@"Reported_Issues")]
            public float Reported_Issues { get; set; }

            [ColumnName(@"Vehicle_Age")]
            public float Vehicle_Age { get; set; }

            [ColumnName(@"Fuel_Type")]
            public float[] Fuel_Type { get; set; }

            [ColumnName(@"Transmission_Type")]
            public float[] Transmission_Type { get; set; }

            [ColumnName(@"Engine_Size")]
            public float Engine_Size { get; set; }

            [ColumnName(@"Odometer_Reading")]
            public float Odometer_Reading { get; set; }

            [ColumnName(@"Last_Service_Date")]
            public float[] Last_Service_Date { get; set; }

            [ColumnName(@"Warranty_Expiry_Date")]
            public float[] Warranty_Expiry_Date { get; set; }

            [ColumnName(@"Owner_Type")]
            public float[] Owner_Type { get; set; }

            [ColumnName(@"Insurance_Premium")]
            public float Insurance_Premium { get; set; }

            [ColumnName(@"Service_History")]
            public float Service_History { get; set; }

            [ColumnName(@"Accident_History")]
            public float Accident_History { get; set; }

            [ColumnName(@"Fuel_Efficiency")]
            public float Fuel_Efficiency { get; set; }

            [ColumnName(@"Tire_Condition")]
            public float[] Tire_Condition { get; set; }

            [ColumnName(@"Brake_Condition")]
            public float[] Brake_Condition { get; set; }

            [ColumnName(@"Battery_Status")]
            public float[] Battery_Status { get; set; }

            [ColumnName(@"Need_Maintenance")]
            public float Need_Maintenance { get; set; }

            [ColumnName(@"Features")]
            public float[] Features { get; set; }

            [ColumnName(@"Score")]
            public float Score { get; set; }

        }

        #endregion

        private static string MLNetModelPath = Path.GetFullPath("MaintenancePredict.mlnet");

        public static readonly Lazy<PredictionEngine<ModelInput, ModelOutput>> PredictEngine = new Lazy<PredictionEngine<ModelInput, ModelOutput>>(() => CreatePredictEngine(), true);


        private static PredictionEngine<ModelInput, ModelOutput> CreatePredictEngine()
        {
            var mlContext = new MLContext();
            ITransformer mlModel = mlContext.Model.Load(MLNetModelPath, out var _);
            return mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);
        }

        public static ModelOutput Predict(ModelInput input)
        {
            var predEngine = PredictEngine.Value;
            return predEngine.Predict(input);
        }
    }
}
