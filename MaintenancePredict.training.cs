using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Trainers.FastTree;
using Microsoft.ML.Transforms;

namespace Kish_AndreiCezar_Project
{
    public partial class MaintenancePredict
    {
        public const string RetrainFilePath =  @"C:\Users\andre\OneDrive\Desktop\Master\Anul 1\Semestrul 1\Dezvoltarea de sisteme de Big Data\Project\vehicle_maintenance_data.csv";
        public const char RetrainSeparatorChar = ',';
        public const bool RetrainHasHeader =  true;
        public const bool RetrainAllowQuoting =  false;

        public static void Train(string outputModelPath, string inputDataFilePath = RetrainFilePath, char separatorChar = RetrainSeparatorChar, bool hasHeader = RetrainHasHeader, bool allowQuoting = RetrainAllowQuoting)
        {
            var mlContext = new MLContext();

            var data = LoadIDataViewFromFile(mlContext, inputDataFilePath, separatorChar, hasHeader, allowQuoting);
            var model = RetrainModel(mlContext, data);
            SaveModel(mlContext, model, data, outputModelPath);
        }

        public static IDataView LoadIDataViewFromFile(MLContext mlContext, string inputDataFilePath, char separatorChar, bool hasHeader, bool allowQuoting)
        {
            return mlContext.Data.LoadFromTextFile<ModelInput>(inputDataFilePath, separatorChar, hasHeader, allowQuoting: allowQuoting);
        }

        public static void SaveModel(MLContext mlContext, ITransformer model, IDataView data, string modelSavePath)
        {
            DataViewSchema dataViewSchema = data.Schema;

            using (var fs = File.Create(modelSavePath))
            {
                mlContext.Model.Save(model, dataViewSchema, fs);
            }
        }

        public static ITransformer RetrainModel(MLContext mlContext, IDataView trainData)
        {
            var pipeline = BuildPipeline(mlContext);
            var model = pipeline.Fit(trainData);

            return model;
        }

        public static IEstimator<ITransformer> BuildPipeline(MLContext mlContext)
        {
            var pipeline = mlContext.Transforms.Categorical.OneHotEncoding(new []{new InputOutputColumnPair(@"Maintenance_History", @"Maintenance_History"),new InputOutputColumnPair(@"Fuel_Type", @"Fuel_Type"),new InputOutputColumnPair(@"Transmission_Type", @"Transmission_Type"),new InputOutputColumnPair(@"Owner_Type", @"Owner_Type"),new InputOutputColumnPair(@"Tire_Condition", @"Tire_Condition"),new InputOutputColumnPair(@"Brake_Condition", @"Brake_Condition"),new InputOutputColumnPair(@"Battery_Status", @"Battery_Status")}, outputKind: OneHotEncodingEstimator.OutputKind.Indicator)      
                                    .Append(mlContext.Transforms.ReplaceMissingValues(new []{new InputOutputColumnPair(@"Mileage", @"Mileage"),new InputOutputColumnPair(@"Reported_Issues", @"Reported_Issues"),new InputOutputColumnPair(@"Vehicle_Age", @"Vehicle_Age"),new InputOutputColumnPair(@"Engine_Size", @"Engine_Size"),new InputOutputColumnPair(@"Odometer_Reading", @"Odometer_Reading"),new InputOutputColumnPair(@"Insurance_Premium", @"Insurance_Premium"),new InputOutputColumnPair(@"Service_History", @"Service_History"),new InputOutputColumnPair(@"Accident_History", @"Accident_History"),new InputOutputColumnPair(@"Fuel_Efficiency", @"Fuel_Efficiency")}))      
                                    .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"Vehicle_Model",outputColumnName:@"Vehicle_Model"))      
                                    .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"Last_Service_Date",outputColumnName:@"Last_Service_Date"))      
                                    .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"Warranty_Expiry_Date",outputColumnName:@"Warranty_Expiry_Date"))      
                                    .Append(mlContext.Transforms.Concatenate(@"Features", new []{@"Maintenance_History",@"Fuel_Type",@"Transmission_Type",@"Owner_Type",@"Tire_Condition",@"Brake_Condition",@"Battery_Status",@"Mileage",@"Reported_Issues",@"Vehicle_Age",@"Engine_Size",@"Odometer_Reading",@"Insurance_Premium",@"Service_History",@"Accident_History",@"Fuel_Efficiency",@"Vehicle_Model",@"Last_Service_Date",@"Warranty_Expiry_Date"}))      
                                    .Append(mlContext.Regression.Trainers.FastForest(new FastForestRegressionTrainer.Options(){NumberOfTrees=8,NumberOfLeaves=4,FeatureFraction=0.9985319F,LabelColumnName=@"Need_Maintenance",FeatureColumnName=@"Features"}));

            return pipeline;
        }
    }
 }
