namespace ServicePerfectCV.Application.Configurations
{
    public class PaymentUrlSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string FrontendBaseUrl { get; set; } = string.Empty;
        public string SuccessEndpoint { get; set; } = string.Empty;
        public string CancelEndpoint { get; set; } = string.Empty;
        public string FrontendSuccessPage { get; set; } = string.Empty;
        public string FrontendCancelPage { get; set; } = string.Empty;

        // Methods for PaymentService
        public string GetReturnUrl() => $"{BaseUrl}{SuccessEndpoint}";
        public string GetCancelUrl() => $"{BaseUrl}/{CancelEndpoint}";

        // Computed properties for full URLs (for other uses)
        public string ReturnUrl => $"{BaseUrl}{SuccessEndpoint}";
        public string CancelUrl => $"{BaseUrl}/{CancelEndpoint}";
        public string FrontendSuccessUrl => $"{FrontendBaseUrl}{FrontendSuccessPage}";
        public string FrontendCancelUrl => $"{FrontendBaseUrl}{FrontendCancelPage}";
    }
}