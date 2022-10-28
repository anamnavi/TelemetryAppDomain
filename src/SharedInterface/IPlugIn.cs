using System;

namespace CommonInterface
{
    public enum TelemetryOperation
    {
        None,
        SearchModule,
        SearchScript,
        Download
    }

    public interface IPlugIn
    {
        string Name { get; }
        void Initialize();

        int Multiply2Nums(int num1,int num2);

        bool OtelStartUp();

        bool OtelTakedown();

        bool OtelProcess(TelemetryOperation operationType, long success, string packageName, string packageVersion, bool statusSuccess, int statusCode, string exceptionType, long duration);
    }
}