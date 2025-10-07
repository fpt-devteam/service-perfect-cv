using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Application.Interfaces.Repositories;
using System;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Services
{
    public class NotificationService
    {
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IDeviceTokenRepository _deviceTokenRepository;

        public NotificationService(
            IPushNotificationService pushNotificationService,
            IDeviceTokenRepository deviceTokenRepository)
        {
            _pushNotificationService = pushNotificationService;
            _deviceTokenRepository = deviceTokenRepository;
        }

        public async Task SendCVUpdateNotificationAsync(Guid userId, string sectionName, string action = "updated")
        {
            try
            {
                var deviceTokens = await _deviceTokenRepository.GetTokensByUserIdAsync(userId);
                if (!deviceTokens.Any()) return;

                var title = "CV Updated";
                var message = $"Your {sectionName} has been {action} successfully.";

                await _pushNotificationService.SendAsync(deviceTokens, title, message);
            }
            catch (Exception ex)
            {
                // Log error but don't throw to avoid breaking the main operation
                // In production, you might want to use a proper logger here
                Console.WriteLine($"Failed to send notification: {ex.Message}");
            }
        }

        public async Task SendContactUpdateNotificationAsync(Guid userId)
        {
            await SendCVUpdateNotificationAsync(userId, "contact information");
        }

        public async Task SendSummaryUpdateNotificationAsync(Guid userId)
        {
            await SendCVUpdateNotificationAsync(userId, "professional summary");
        }

        public async Task SendEducationUpdateNotificationAsync(Guid userId, string action = "updated")
        {
            await SendCVUpdateNotificationAsync(userId, "education", action);
        }

        public async Task SendExperienceUpdateNotificationAsync(Guid userId, string action = "updated")
        {
            await SendCVUpdateNotificationAsync(userId, "work experience", action);
        }

        public async Task SendProjectUpdateNotificationAsync(Guid userId, string action = "updated")
        {
            await SendCVUpdateNotificationAsync(userId, "project", action);
        }

        public async Task SendSkillUpdateNotificationAsync(Guid userId, string action = "updated")
        {
            await SendCVUpdateNotificationAsync(userId, "skill", action);
        }

        public async Task SendCertificationUpdateNotificationAsync(Guid userId, string action = "updated")
        {
            await SendCVUpdateNotificationAsync(userId, "certification", action);
        }
    }
}