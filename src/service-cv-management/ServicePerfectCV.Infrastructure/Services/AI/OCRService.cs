using Azure;
using Azure.AI.DocumentIntelligence;
using Microsoft.Extensions.Logging;
using ServicePerfectCV.Application.Interfaces.AI;
using System.Text;

namespace ServicePerfectCV.Infrastructure.Services.AI
{
    /// <summary>
    /// OCR service using Azure Document Intelligence for extracting text from PDF files
    /// </summary>
    public class OCRService : IOCRService
    {
        private readonly DocumentIntelligenceClient _client;
        private readonly ILogger<OCRService> _logger;

        public OCRService(
            DocumentIntelligenceClient client,
            ILogger<OCRService> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Extracts text from a PDF file using Azure Document Intelligence
        /// </summary>
        /// <param name="pdfBytes">The PDF file content as byte array</param>
        /// <param name="fileName">Optional file name for logging purposes</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Extracted text content from the PDF</returns>
        public async Task<string> ExtractTextFromPdfAsync(
            byte[] pdfBytes,
            string? fileName = null,
            CancellationToken cancellationToken = default)
        {
            if (pdfBytes == null || pdfBytes.Length == 0)
                throw new ArgumentException("PDF content cannot be null or empty", nameof(pdfBytes));

            try
            {
                _logger.LogInformation("Starting OCR extraction for PDF file: {FileName} ({Size} bytes)",
                    fileName ?? "unknown", pdfBytes.Length);

                using var stream = new MemoryStream(pdfBytes);
                return await ExtractTextFromPdfAsync(stream, fileName, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to extract text from PDF file: {FileName}", fileName ?? "unknown");
                throw;
            }
        }

        /// <summary>
        /// Extracts text from a PDF file stream using Azure Document Intelligence
        /// </summary>
        /// <param name="pdfStream">The PDF file stream</param>
        /// <param name="fileName">Optional file name for logging purposes</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Extracted text content from the PDF</returns>
        public async Task<string> ExtractTextFromPdfAsync(
            Stream pdfStream,
            string? fileName = null,
            CancellationToken cancellationToken = default)
        {
            if (pdfStream == null)
                throw new ArgumentNullException(nameof(pdfStream));

            if (!pdfStream.CanRead)
                throw new ArgumentException("Stream must be readable", nameof(pdfStream));

            try
            {
                _logger.LogInformation("Starting OCR extraction from stream for PDF file: {FileName}", fileName ?? "unknown");

                // Ensure the stream is at the beginning
                if (pdfStream.CanSeek)
                {
                    pdfStream.Position = 0;
                }

                // Convert stream to BinaryData for the API
                var binaryData = await BinaryData.FromStreamAsync(pdfStream, cancellationToken);

                // Start the analysis operation using the prebuilt-read model
                var operation = await _client.AnalyzeDocumentAsync(
                    WaitUntil.Completed,
                    "prebuilt-read",
                    binaryData,
                    cancellationToken: cancellationToken);

                var result = operation.Value;

                // Extract text from the result
                var extractedText = ExtractTextFromResult(result);

                _logger.LogInformation(
                    "Successfully extracted text from PDF file: {FileName}. Extracted {Length} characters from {PageCount} page(s)",
                    fileName ?? "unknown",
                    extractedText.Length,
                    result.Pages.Count);

                return extractedText;
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError(ex,
                    "Azure Document Intelligence API request failed for file: {FileName}. Status: {Status}, Error Code: {ErrorCode}",
                    fileName ?? "unknown",
                    ex.Status,
                    ex.ErrorCode);
                throw new InvalidOperationException(
                    $"Failed to process PDF file with Azure Document Intelligence: {ex.Message}",
                    ex);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("OCR operation was cancelled for file: {FileName}", fileName ?? "unknown");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during OCR extraction for file: {FileName}", fileName ?? "unknown");
                throw;
            }
        }

        /// <summary>
        /// Extracts and formats text from the Document Intelligence analysis result
        /// </summary>
        /// <param name="result">The analysis result from Document Intelligence</param>
        /// <returns>Formatted text content</returns>
        private string ExtractTextFromResult(AnalyzeResult result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            var textBuilder = new StringBuilder();

            // Option 1: Use the Content property (simple, maintains document flow)
            if (!string.IsNullOrEmpty(result.Content))
            {
                _logger.LogDebug("Using result.Content for text extraction");
                return result.Content;
            }

            // Option 2: Extract text from pages and paragraphs (more structured)
            _logger.LogDebug("Extracting text from pages and paragraphs");

            if (result.Pages != null && result.Pages.Count > 0)
            {
                foreach (var page in result.Pages)
                {
                    _logger.LogDebug("Processing page {PageNumber}", page.PageNumber);

                    // Extract text from lines on the page
                    if (page.Lines != null)
                    {
                        foreach (var line in page.Lines)
                        {
                            textBuilder.AppendLine(line.Content);
                        }
                    }

                    // Add page separator for multi-page documents
                    if (page.PageNumber < result.Pages.Count)
                    {
                        textBuilder.AppendLine();
                        textBuilder.AppendLine("---"); // Page separator
                        textBuilder.AppendLine();
                    }
                }
            }

            // Option 3: Use paragraphs if available (best for semantic structure)
            if (result.Paragraphs != null && result.Paragraphs.Count > 0)
            {
                _logger.LogDebug("Found {Count} paragraphs in the document", result.Paragraphs.Count);

                textBuilder.Clear(); // Clear previous content if paragraphs are available
                foreach (var paragraph in result.Paragraphs)
                {
                    if (!string.IsNullOrEmpty(paragraph.Content))
                    {
                        textBuilder.AppendLine(paragraph.Content);
                        textBuilder.AppendLine(); // Add spacing between paragraphs
                    }
                }
            }

            var extractedText = textBuilder.ToString().Trim();

            if (string.IsNullOrEmpty(extractedText))
            {
                _logger.LogWarning("No text content could be extracted from the document");
                return string.Empty;
            }

            return extractedText;
        }
    }
}

