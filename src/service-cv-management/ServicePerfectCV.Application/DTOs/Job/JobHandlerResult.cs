using System.Text.Json;

namespace ServicePerfectCV.Application.Services.Jobs
{
    public sealed class JobHandlerResult
    {
        private JobHandlerResult(bool succeeded, JsonDocument? output, string? errorCode, string? errorMessage)
        {
            Succeeded = succeeded;
            Output = output;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public bool Succeeded { get; }
        public JsonDocument? Output { get; }
        public string? ErrorCode { get; }
        public string? ErrorMessage { get; }

        public static JobHandlerResult Success(JsonDocument output)
        {
            return new JobHandlerResult(true, output, null, null);
        }

        public static JobHandlerResult Failure(string? errorCode, string? errorMessage, JsonDocument? output = null)
        {
            return new JobHandlerResult(false, output, errorCode, errorMessage);
        }
    }
}
