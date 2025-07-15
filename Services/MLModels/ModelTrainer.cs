using System;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace Services.MLModels
{
    public class BookingData
    {
        [LoadColumn(0)]
        public float DayOfWeek { get; set; }

        [LoadColumn(1)]
        public float TimeOfDay { get; set; }

        [LoadColumn(2)]
        public float Court { get; set; }

        [LoadColumn(3)]
        public float NumberOfSlot { get; set; }

        [LoadColumn(4)]
        public float BookingCount { get; set; }
    }

    public class BookingPrediction
    {
        [ColumnName("Score")]
        public float BookingCount { get; set; }
    }

    public class ModelTrainer
    {
        private readonly string _trainDataPath;
        private readonly string _modelPath;
        private readonly MLContext _mlContext;

        public ModelTrainer(string trainDataPath, string modelPath)
        {
            _trainDataPath = trainDataPath;
            _modelPath = modelPath;
            _mlContext = new MLContext();
        }

        public void TrainAndSaveModel()
        {
            try
            {
                Console.WriteLine("Current Directory: " + Environment.CurrentDirectory);
                Console.WriteLine("Training data path: " + _trainDataPath);

                if (!File.Exists(_trainDataPath))
                {
                    Console.WriteLine($"Training data file not found: {_trainDataPath}");
                    throw new FileNotFoundException(
                        "Training data file not found.",
                        _trainDataPath
                    );
                }

                string baseDir = Path.GetDirectoryName(_modelPath);
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string runDir = Path.Combine(baseDir, $"ModelRun_{timestamp}");
                Directory.CreateDirectory(runDir);
                string modelPath = Path.Combine(runDir, "Model.zip");
                string metricsPath = Path.Combine(runDir, "ModelMetrics.txt");

                // Load data
                IDataView dataView = _mlContext.Data.LoadFromTextFile<BookingData>(
                    _trainDataPath,
                    hasHeader: true,
                    separatorChar: ','
                );
                var rowCount = dataView.GetRowCount();
                Console.WriteLine("Number of rows in training data: " + rowCount);
                if (rowCount == 0)
                {
                    throw new InvalidOperationException(
                        "Training set has 0 instances, aborting training."
                    );
                }

                // Build pipeline with normalization
                var pipeline = _mlContext
                    .Transforms.CopyColumns(
                        outputColumnName: "Label",
                        inputColumnName: "BookingCount"
                    )
                    .Append(
                        _mlContext.Transforms.Concatenate(
                            "Features",
                            "DayOfWeek",
                            "TimeOfDay",
                            "Court",
                            "NumberOfSlot"
                        )
                    )
                    .Append(_mlContext.Transforms.NormalizeMinMax("Features", "Features"))
                    .AppendCacheCheckpoint(_mlContext)
                    .Append(
                        _mlContext.Regression.Trainers.LightGbm(
                            new Microsoft.ML.Trainers.LightGbm.LightGbmRegressionTrainer.Options
                            {
                                NumberOfIterations = 100,
                                LearningRate = 0.1,
                                NumberOfLeaves = 31,
                                MinimumExampleCountPerLeaf = 10,
                            }
                        )
                    );

                // Split data: 80% train, 20% test
                var split = _mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
                Console.WriteLine(
                    $"Train set: {split.TrainSet.GetRowCount()} rows, Test set: {split.TestSet.GetRowCount()} rows"
                );

                // Train model on train set
                var model = pipeline.Fit(split.TrainSet);

                // Evaluate on test set
                Console.WriteLine("--- Hold-out Test Set Evaluation ---");
                var testMetrics = EvaluateAndReturn(model, split.TestSet);

                // Save model
                SaveModel(model, split.TrainSet.Schema, modelPath);

                // K-fold cross-validation
                Console.WriteLine("--- K-Fold Cross-Validation (5 folds) ---");
                var cvResults = _mlContext.Regression.CrossValidate(
                    dataView,
                    pipeline,
                    numberOfFolds: 5
                );
                double avgR2 = cvResults.Average(fold => fold.Metrics.RSquared);
                double avgRMSE = cvResults.Average(fold => fold.Metrics.RootMeanSquaredError);

                // Ghi metrics ra file
                var metricsReport = new System.Text.StringBuilder();
                metricsReport.AppendLine($"Train/Test split evaluation:");
                metricsReport.AppendLine($"R^2: {testMetrics.RSquared:0.###}");
                metricsReport.AppendLine($"RMSE: {testMetrics.RootMeanSquaredError:0.###}");
                metricsReport.AppendLine();
                metricsReport.AppendLine($"K-Fold Cross-Validation (5 folds):");
                for (int i = 0; i < cvResults.Count; i++)
                {
                    metricsReport.AppendLine(
                        $"Fold {i + 1}: R^2 = {cvResults[i].Metrics.RSquared:0.###}, RMSE = {cvResults[i].Metrics.RootMeanSquaredError:0.###}"
                    );
                }
                metricsReport.AppendLine($"Average R^2: {avgR2:0.###}");
                metricsReport.AppendLine($"Average RMSE: {avgRMSE:0.###}");
                metricsReport.AppendLine();
                metricsReport.AppendLine($"Total rows: {rowCount}");
                metricsReport.AppendLine($"Train rows: {split.TrainSet.GetRowCount()}");
                metricsReport.AppendLine($"Test rows: {split.TestSet.GetRowCount()}");
                metricsReport.AppendLine($"Training time: {DateTime.Now}");

                File.WriteAllText(metricsPath, metricsReport.ToString());
                Console.WriteLine($"Model and metrics saved in: {runDir}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during model training: {ex.Message}");
                throw;
            }
        }

        private RegressionMetrics EvaluateAndReturn(ITransformer model, IDataView dataView)
        {
            var predictions = model.Transform(dataView);
            var metrics = _mlContext.Regression.Evaluate(predictions, "Label", "Score");
            Console.WriteLine($"R^2: {metrics.RSquared:0.###}");
            Console.WriteLine($"RMS error: {metrics.RootMeanSquaredError:#.###}");
            return metrics;
        }

        private void CrossValidate(IEstimator<ITransformer> pipeline, IDataView data, int k)
        {
            var cvResults = _mlContext.Regression.CrossValidate(data, pipeline, numberOfFolds: k);
            double avgR2 = cvResults.Average(fold => fold.Metrics.RSquared);
            double avgRMSE = cvResults.Average(fold => fold.Metrics.RootMeanSquaredError);
            for (int i = 0; i < cvResults.Count; i++)
            {
                Console.WriteLine(
                    $"Fold {i + 1}: R^2 = {cvResults[i].Metrics.RSquared:0.###}, RMSE = {cvResults[i].Metrics.RootMeanSquaredError:0.###}"
                );
            }
            Console.WriteLine(
                $"K-Fold Cross-Validation ({k} folds): Avg R^2 = {avgR2:0.###}, Avg RMSE = {avgRMSE:0.###}"
            );
        }

        private void SaveModel(ITransformer model, DataViewSchema schema, string path)
        {
            using (
                var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write)
            )
            {
                _mlContext.Model.Save(model, schema, fs);
            }
            Console.WriteLine($"The model is saved to {path}");
        }
    }
}
