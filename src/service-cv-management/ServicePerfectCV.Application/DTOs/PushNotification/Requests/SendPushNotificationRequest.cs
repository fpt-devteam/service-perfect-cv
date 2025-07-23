namespace ServicePerfectCV.Application.DTOs.PushNotification.Requests
{
    public class SendPushNotificationRequest
    {
        public IEnumerable<string> DeviceTokens { get; init; } = new List<string>();
        public string Title { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
    }
}