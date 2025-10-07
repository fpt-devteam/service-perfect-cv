using ServicePerfectCV.Application.DTOs.CvStructuring;

namespace ServicePerfectCV.Application.Interfaces.AI
{
    /// <summary>
    /// Service for structuring CV raw text and extracting structured sections using LLM
    /// </summary>
    public interface ICvStructuringService
    {
        /// <summary>
        /// Structures raw CV text and extracts all sections as structured data
        /// </summary>
        /// <param name="rawText">The raw text extracted from CV</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Dictionary mapping section types to their JSON representations</returns>
        Task<CvSectionExtractionResult> StructureCvContentAsync(string rawText, CancellationToken cancellationToken = default);
    }
}
