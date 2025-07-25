namespace ServicePerfectCV.Application.Configurations
{
    public class FirebaseCloudStorageSettings
    {
        public required string Type { get; set; }
        public required string ProjectId { get; set; }
        public required string PrivateKeyId { get; set; }
        public required string PrivateKey { get; set; }
        public required string ClientEmail { get; set; }
        public required string ClientId { get; set; }
        public required string AuthUri { get; set; }
        public required string TokenUri { get; set; }
        public required string AuthProviderX509CertUrl { get; set; }
        public required string ClientX509CertUrl { get; set; }
        public required string UniverseDomain { get; set; }
        public required string BucketName { get; set; }
        public required string StorageUrl { get; set; }
    }
}
