namespace ServicePerfectCV.Application.Configurations
{
    public class AzureStorageSettings
    {
        public required string ConnectionString { get; set; }
        public required string ContainerName { get; set; }
        public required string FcmBlobName { get; set; }
        public required string FirebaseStorageServiceBlobName { get; set; }
    }
}