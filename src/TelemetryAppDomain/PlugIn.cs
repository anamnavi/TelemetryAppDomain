using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OpenTelemetry.Exporter.Geneva;
using System.Diagnostics.Metrics;
using NuGetGallery;

namespace PlugIn1
{
    [Serializable]
    public class PlugIn : MarshalByRefObject, IMyPlugIn
    {
        private static IDisposable _meterProvider;
        private static IDisposable _tracerProvider;

        private const string PackageNameMetricsKey = "PkgName";
        private const string PackageVersionMetricsKey = "PkgVersion";
        private const string SuccessMetricsKey = "Success";
        private const string StatusCodeMetricsKey = "StatusCode";
        private const string ExceptionTypeMetricsKey = "ExceptionType";
        private const string DurationMetricsKey = "Duration";
        private const string OperationKey = "Operation";
        private const string HttpStatusCodeKey = "HttpStatusCode";
        private static readonly Meter psGalleryTelemetryMeter = new Meter("PowerShellGalleryMetrics", "1.0");
        private static readonly Counter<long> downloadCounter = psGalleryTelemetryMeter.CreateCounter<long>("DownloadMetric");
        private static readonly Counter<long> searchModuleCounter = psGalleryTelemetryMeter.CreateCounter<long>("SearchMetricModules");
        private static readonly Counter<long> searchScriptCounter = psGalleryTelemetryMeter.CreateCounter<long>("SearchMetricScripts");
        private static readonly Histogram<long> downloadResponseLatencyHistogram = psGalleryTelemetryMeter.CreateHistogram<long>("DownloadResponseLatencyMs");
        private static readonly Histogram<long> searchModuleResponseLatencyHistogram = psGalleryTelemetryMeter.CreateHistogram<long>("SearchModulesResponseLatencyMs");
        private static readonly Histogram<long> searchScriptResponseLatencyHistogram = psGalleryTelemetryMeter.CreateHistogram<long>("SearchScriptsResponseLatencyMs");

        private string name;

        public string Name { get { return name; } }

        public void Initialize()
        {
            name = "PlugIn 1";
        }

        public int Multiply2Nums(int num1, int num2)
        {
            return num1 * num2;
        }

        public bool OtelStartUp()
        {
            // AppActivator.AppPostStart() equivalent code:
            // Create MeterProvider, TracerProvider instances

            // Create a double[] to provide custom bucket bounds for the histogram
            // By default, OpenTelemetry SDK uses [ 0, 5, 10, 25, 50, 75, 100, 250, 500, 1000 ] as the bucket values as per the OpenTelemetry specification
            // https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/metrics/sdk.md#explicit-bucket-histogram-aggregation
            var minValue = 10;
            var bucketSize = 10;
            var bucketCount = 2000;
            var firstBucketValue = minValue;
            var customBucketBounds = new double[bucketCount];
            for (int i = 0; i < bucketCount; i++)
            {
                customBucketBounds[i] = firstBucketValue;
                firstBucketValue += bucketSize;
            }

            var searchModuleHistogramConfig = new ExplicitBucketHistogramConfiguration();
            searchModuleHistogramConfig.Boundaries = customBucketBounds;

            var searchScriptHistogramConfig = new ExplicitBucketHistogramConfiguration();
            searchScriptHistogramConfig.Boundaries = customBucketBounds;

            var downloadHistogramConfig = new ExplicitBucketHistogramConfiguration();
            downloadHistogramConfig.Boundaries = customBucketBounds;

            var builder = Sdk.CreateTracerProviderBuilder()
                .AddAspNetInstrumentation()
                .AddHttpClientInstrumentation();

            _tracerProvider = builder.Build();

            var genevaMeterProvider = Sdk.CreateMeterProviderBuilder()
                .AddAspNetInstrumentation()
                .AddMeter("PowerShellGalleryMetrics")
                .AddView("SearchModulesResponseLatencyMs", searchModuleHistogramConfig)
                .AddView("SearchScriptsResponseLatencyMs", searchScriptHistogramConfig)
                .AddView("DownloadResponseLatencyMs", downloadHistogramConfig)
                .AddGenevaMetricExporter(options =>
                {
                    options.ConnectionString = "Account=PowerShellGalleryHotPath;Namespace=TestSLINamespace";
                    options.PrepopulatedMetricDimensions = new ReadOnlyDictionary<string, Object>(
                        new Dictionary<string, Object>()
                        {
                            ["CustomerResourceId"] = "defaultuser",
                            ["LocationId"] = "Psgintcentralus",
                            ["EnvironmentId"] = "Psgintcentralus_Env"
                        });
                });

            _meterProvider = genevaMeterProvider.Build();

            return true;
        }

        public bool OtelTakedown()
        {
            // AppActivator.BackgroundJobsStop() code equivalent
            // Dispose of MeterProvider and TracerProvider objects
            if (_tracerProvider != null)
            {
                _tracerProvider.Dispose();
            }

            if (_meterProvider != null)
            {
                _meterProvider.Dispose();
            }

            return true;
        }

        public bool OtelProcess(TelemetryOperation operationType, long success, string packageName, string packageVersion, bool statusSuccess, int statusCode, string exceptionType, long duration)
        {
            if (operationType == TelemetryOperation.Download)
            {
                OtelProcessRecordHelper(downloadCounter, downloadResponseLatencyHistogram, success, packageName, packageVersion, statusSuccess, statusCode, exceptionType, duration);
            }
            else if (operationType == TelemetryOperation.SearchModule)
            {
                OtelProcessRecordHelper(searchModuleCounter, searchModuleResponseLatencyHistogram, success, packageName, packageVersion, statusSuccess, statusCode, exceptionType, duration);
            }
            else if (operationType == TelemetryOperation.SearchScript)
            {
                OtelProcessRecordHelper(searchScriptCounter, searchScriptResponseLatencyHistogram, success, packageName, packageVersion, statusSuccess, statusCode, exceptionType, duration);
            }

            return true;
        }

        public bool OtelProcessRecordHelper(Counter<long> operationCounter,
            Histogram<long> operationHistogram,
            long success,
            string packageName,
            string packageVersion,
            bool statusSuccess,
            int statusCode,
            string exceptionType,
            long duration)
        {
            if (String.IsNullOrEmpty(exceptionType))
            {
                operationCounter.Add(success,
                    new KeyValuePair<string, Object>(PackageNameMetricsKey, packageName),
                    new KeyValuePair<string, Object>(PackageVersionMetricsKey, packageVersion),
                    new KeyValuePair<string, Object>(SuccessMetricsKey, statusSuccess),
                    new KeyValuePair<string, Object>(StatusCodeMetricsKey, statusCode),
                    new KeyValuePair<string, Object>(DurationMetricsKey, duration));
            }
            else
            {
                operationCounter.Add(success,
                    new KeyValuePair<string, Object>(PackageNameMetricsKey, packageName),
                    new KeyValuePair<string, Object>(PackageVersionMetricsKey, packageVersion),
                    new KeyValuePair<string, Object>(SuccessMetricsKey, statusSuccess),
                    new KeyValuePair<string, Object>(StatusCodeMetricsKey, statusCode),
                    new KeyValuePair<string, Object>(ExceptionTypeMetricsKey, exceptionType),
                    new KeyValuePair<string, Object>(DurationMetricsKey, duration));
            }


            // Record the latency result of an http operation and update the latency histogram
            operationHistogram.Record(
                duration,
                new KeyValuePair<string, Object>(OperationKey, "GET"),
                new KeyValuePair<string, Object>(HttpStatusCodeKey, statusCode));

            return true;
        }
    }
}