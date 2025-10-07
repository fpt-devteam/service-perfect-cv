using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Interfaces.AI;
using ServicePerfectCV.Application.Interfaces.Repositories;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ServicePerfectCV.WebApi.Controllers
{
    [ApiController]
    [Route("api/ocr")]
    public class OCRController : ControllerBase
    {
        private readonly IOCRService _ocrService;
        private readonly ICVRepository _cvRepository;
        private readonly ILogger<OCRController> _logger;

        public OCRController(
            IOCRService ocrService,
            ICVRepository cvRepository,
            ILogger<OCRController> logger)
        {
            _ocrService = ocrService ?? throw new ArgumentNullException(nameof(ocrService));
            _cvRepository = cvRepository ?? throw new ArgumentNullException(nameof(cvRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("test")]
        [ProducesResponseType(typeof(OCRTestResponse), 200)]
        public async Task<IActionResult> TestOCRWithCV(
            CancellationToken cancellationToken = default)
        {
            Guid cvId = new Guid("9574ff49-f399-48d6-9741-6fe0c2d576e5"); // For testing purposes, replace with actual input
            Guid userId = new Guid("144bb49f-b790-459e-b6c5-628577c4a8f2"); // For testing purposes, replace with actual user context
            try
            {
                var startTime = DateTime.UtcNow;
                _logger.LogInformation("Starting OCR test for CV {CvId} by user {UserId}", cvId, userId);

                // Get CV by ID and user ID (ensures authorization)
                var cv = await _cvRepository.GetByCVIdAndUserIdAsync(cvId, userId);
                if (cv == null)
                {
                    _logger.LogWarning("CV {CvId} not found for user {UserId}", cvId, userId);
                    return NotFound($"CV with ID {cvId} not found or you don't have permission to access it");
                }

                // Validate that CV has PDF file
                if (cv.PdfFile == null || cv.PdfFile.Length == 0)
                {
                    _logger.LogWarning("CV {CvId} has no PDF file attached", cvId);
                    return BadRequest("CV does not have a PDF file attached for OCR processing");
                }

                _logger.LogInformation("Processing PDF file {FileName} ({Size} bytes) for CV {CvId}",
                    cv.PdfFileName ?? "unknown", cv.PdfFile.Length, cvId);

                // Process PDF with OCR service
                var extractedText = await _ocrService.ExtractTextFromPdfAsync(
                    cv.PdfFile,
                    cv.PdfFileName,
                    cancellationToken);

                var endTime = DateTime.UtcNow;
                var processingTime = (endTime - startTime).TotalMilliseconds;

                _logger.LogInformation("OCR processing completed for CV {CvId}. Extracted {TextLength} characters in {ProcessingTime}ms",
                    cvId, extractedText?.Length ?? 0, processingTime);

                var response = new OCRTestResponse
                {
                    CvId = cvId,
                    FileName = cv.PdfFileName,
                    ExtractedText = extractedText,
                    ExtractedAt = endTime,
                    ProcessingTimeMs = (int)processingTime
                };

                return Ok(response);
            }
            catch (DomainException ex)
            {
                _logger.LogWarning(ex, "Domain exception during OCR test for CV {CvId}: {ErrorCode}", cvId, ex.Error.Code);
                return StatusCode((int)ex.Error.HttpStatusCode, ex.Error.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument during OCR test for CV {CvId}", cvId);
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access during OCR test for CV {CvId}", cvId);
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during OCR test for CV {CvId}", cvId);
                return StatusCode(500, "An unexpected error occurred during OCR processing");
            }
        }

        /// <summary>
        /// Tests OCR service with a directly uploaded PDF file
        /// </summary>
        /// <param name="pdfFile">The PDF file to process with OCR</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The extracted text from the PDF file</returns>
        /// <remarks>
        /// This endpoint allows direct testing of the OCR service without requiring an existing CV.
        /// Upload a PDF file and get the extracted text content.
        /// 
        /// Requirements:
        /// - User must be authenticated
        /// - File must be a PDF (checked by content type and extension)
        /// - File size must be reasonable (max 10MB)
        /// 
        /// Example response:
        /// ```json
        /// {
        ///   "fileName": "test-resume.pdf",
        ///   "fileSize": 524288,
        ///   "extractedText": "John Doe\nSoftware Developer\n...",
        ///   "extractedAt": "2023-10-06T10:30:00Z",
        ///   "processingTimeMs": 1800
        /// }
        /// ```
        /// </remarks>
        /// <response code="200">Returns the extracted text from the PDF</response>
        /// <response code="400">Invalid file or request</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="500">OCR processing failed</response>
        [HttpPost("test-upload")]
        [ProducesResponseType(typeof(OCRUploadTestResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> TestOCRWithUpload(
            [FromForm] IFormFile pdfFile,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var startTime = DateTime.UtcNow;

                // Validate file
                if (pdfFile == null || pdfFile.Length == 0)
                {
                    return BadRequest("PDF file is required");
                }

                // Check file size (max 10MB)
                const int maxFileSize = 10 * 1024 * 1024; // 10MB
                if (pdfFile.Length > maxFileSize)
                {
                    return BadRequest($"File size exceeds maximum limit of {maxFileSize / (1024 * 1024)}MB");
                }

                // Validate file type
                var allowedContentTypes = new[] { "application/pdf" };
                if (!allowedContentTypes.Contains(pdfFile.ContentType.ToLowerInvariant()))
                {
                    return BadRequest("Only PDF files are supported");
                }

                // Validate file extension
                var fileExtension = Path.GetExtension(pdfFile.FileName)?.ToLowerInvariant();
                if (fileExtension != ".pdf")
                {
                    return BadRequest("File must have a .pdf extension");
                }

                _logger.LogInformation("Processing uploaded PDF file {FileName} ({Size} bytes)",
                    pdfFile.FileName, pdfFile.Length);

                // Convert file to byte array
                byte[] pdfBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await pdfFile.CopyToAsync(memoryStream, cancellationToken);
                    pdfBytes = memoryStream.ToArray();
                }

                // Process PDF with OCR service
                var extractedText = await _ocrService.ExtractTextFromPdfAsync(
                    pdfBytes,
                    pdfFile.FileName,
                    cancellationToken);

                var endTime = DateTime.UtcNow;
                var processingTime = (endTime - startTime).TotalMilliseconds;

                _logger.LogInformation("OCR processing completed for uploaded file {FileName}. Extracted {TextLength} characters in {ProcessingTime}ms",
                    pdfFile.FileName, extractedText?.Length ?? 0, processingTime);

                var response = new OCRUploadTestResponse
                {
                    FileName = pdfFile.FileName,
                    FileSize = pdfFile.Length,
                    ExtractedText = extractedText,
                    ExtractedAt = endTime,
                    ProcessingTimeMs = (int)processingTime
                };

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument during OCR upload test");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during OCR upload test for file {FileName}", pdfFile?.FileName);
                return StatusCode(500, "An unexpected error occurred during OCR processing");
            }
        }
    }

    /// <summary>
    /// Response model for OCR test with CV
    /// </summary>
    public class OCRTestResponse
    {
        /// <summary>
        /// The CV ID that was processed
        /// </summary>
        public Guid CvId { get; set; }

        /// <summary>
        /// The original PDF file name
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// The extracted text content from the PDF
        /// </summary>
        public string? ExtractedText { get; set; }

        /// <summary>
        /// Timestamp when the text was extracted
        /// </summary>
        public DateTime ExtractedAt { get; set; }

        /// <summary>
        /// Processing time in milliseconds
        /// </summary>
        public int ProcessingTimeMs { get; set; }
    }

    /// <summary>
    /// Response model for OCR test with uploaded file
    /// </summary>
    public class OCRUploadTestResponse
    {
        /// <summary>
        /// The uploaded PDF file name
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// The size of the uploaded file in bytes
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// The extracted text content from the PDF
        /// </summary>
        public string? ExtractedText { get; set; }

        /// <summary>
        /// Timestamp when the text was extracted
        /// </summary>
        public DateTime ExtractedAt { get; set; }

        /// <summary>
        /// Processing time in milliseconds
        /// </summary>
        public int ProcessingTimeMs { get; set; }
    }
}