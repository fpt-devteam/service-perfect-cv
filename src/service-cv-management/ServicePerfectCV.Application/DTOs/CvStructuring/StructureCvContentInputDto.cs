namespace ServicePerfectCV.Application.DTOs.CvStructuring
{
    /// <summary>
    /// Input DTO for CV content structuring job
    /// </summary>
    public sealed class StructureCvContentInputDto
    {
        /// <summary>
        /// CV ID
        /// </summary>
        public Guid CvId { get; set; }

        /// <summary>
        /// User ID who owns the CV
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Raw text extracted from the CV document
        /// </summary>
        public string RawText { get; set; } = string.Empty;
    }
}
