using Microsoft.ML.Data;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using Plotly.NET;
using Plotly.NET.LayoutObjects;

namespace Kish_AndreiCezar_Project
{
    public partial class MaintenancePredict
    {
        public static List<Tuple<string, double>> CalculatePFI(MLContext mlContext, IDataView trainData, ITransformer model, string labelColumnName)
        {
            var preprocessedTrainData = model.Transform(trainData);

            var permutationFeatureImportance =
         mlContext.Regression
         .PermutationFeatureImportance(
                 model,
                 preprocessedTrainData,
                 labelColumnName: labelColumnName);

            var featureImportanceMetrics =
                 permutationFeatureImportance
                 .Select((kvp) => new { kvp.Key, kvp.Value.RSquared })
                 .OrderByDescending(myFeatures => Math.Abs(myFeatures.RSquared.Mean));

            var featurePFI = new List<Tuple<string, double>>();
            foreach (var feature in featureImportanceMetrics)
            {
                var pfiValue = Math.Abs(feature.RSquared.Mean);
                featurePFI.Add(new Tuple<string, double>(feature.Key, pfiValue));
            }

            return featurePFI;
        }

        public static void PlotRSquaredValues(IDataView trainData, ITransformer model, string labelColumnName, string folderPath)
        {
            int numberOfRows = 1000;
            var testResults = model.Transform(trainData);

            var trueValues = testResults.GetColumn<float>(labelColumnName).Take(numberOfRows); ;

            var predictedValues = testResults.GetColumn<float>("Score").Take(numberOfRows);

            var title = Title.init(Text: "R-Squared Plot");
            var layout = Layout.init<IConvertible>(Title: title, PlotBGColor: Plotly.NET.Color.fromString("#e5ecf6"));
            var xAxis = LinearAxis.init<IConvertible, IConvertible, IConvertible, IConvertible, IConvertible, IConvertible>(
                    Title: Title.init("True Values"),
                    ZeroLineColor: Plotly.NET.Color.fromString("#ffff"),
                    GridColor: Plotly.NET.Color.fromString("#ffff"),
                    ZeroLineWidth: 2);
            var yAxis = LinearAxis.init<IConvertible, IConvertible, IConvertible, IConvertible, IConvertible, IConvertible>(
                    Title: Title.init("Predicted Values"),
                    ZeroLineColor: Plotly.NET.Color.fromString("#ffff"),
                    GridColor: Plotly.NET.Color.fromString("#ffff"),
                    ZeroLineWidth: 2);

            var maximumValue = Math.Max(trueValues.Max(), predictedValues.Max());
            var perfectX = new[] { 0, maximumValue };
            var perfectY = new[] { 0, maximumValue };



            var trueAndPredictedValues = Chart2D.Chart.Scatter<float, float, string>(x: trueValues, y: predictedValues, mode: StyleParam.Mode.Markers)
                            .WithLayout(layout)
                            .WithXAxis(xAxis)
                            .WithYAxis(yAxis);

            var perfectLineGraph = Chart2D.Chart.Line<float, float, string>(x: perfectX, y: perfectY)
                            .WithLayout(layout)
                            .WithLine(Line.init(Width: 1.5));

            var chartWithValuesAndIdealLine = Chart.Combine(new[] { trueAndPredictedValues, perfectLineGraph });
            var chartFilePath = folderPath + "\\RegressionChart.html";

            chartWithValuesAndIdealLine.SaveHtml(chartFilePath);
        }
    }
}


