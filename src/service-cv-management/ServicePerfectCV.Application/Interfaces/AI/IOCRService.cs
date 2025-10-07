namespace ServicePerfectCV.Application.Interfaces.AI
{
    /// <summary>
    /// Service for Optical Character Recognition (OCR) operations
    /// </summary>
    public interface IOCRService
    {
        /// <summary>
        /// Extracts text from a PDF file using OCR
        /// </summary>
        /// <param name="pdfBytes">The PDF file content as byte array</param>
        /// <param name="fileName">Optional file name for logging purposes</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Extracted text content from the PDF</returns>
        Task<string> ExtractTextFromPdfAsync(byte[] pdfBytes, string? fileName = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Extracts text from a PDF file stream using OCR
        /// </summary>
        /// <param name="pdfStream">The PDF file stream</param>
        /// <param name="fileName">Optional file name for logging purposes</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Extracted text content from the PDF</returns>
        Task<string> ExtractTextFromPdfAsync(Stream pdfStream, string? fileName = null, CancellationToken cancellationToken = default);
    }
}

