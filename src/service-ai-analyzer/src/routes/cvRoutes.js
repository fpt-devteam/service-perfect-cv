const express = require("express");
const cvController = require("../controllers/cvController");
const {
  validateCVAnalysis,
  validateAnalysisId,
  validateUserId,
  validateCVId,
} = require("../middleware/validation");
const { rateLimiter } = require("../middleware/rateLimiter");

const router = express.Router();

// CV AI Analysis Routes (called by main CV service)

/**
 * @route   POST /api/cv/analyze
 * @desc    Submit structured CV data for AI analysis (called by main CV service)
 * @access  Internal service (should add service authentication in production)
 */
router.post(
  "/analyze",
  rateLimiter(10), // 10 requests per window
  validateCVAnalysis,
  cvController.submitCVForAnalysis
);

/**
 * @route   GET /api/cv/analysis/:analysisId/status
 * @desc    Get CV analysis processing status
 * @access  Public (in production, add authentication)
 */
router.get(
  "/analysis/:analysisId/status",
  validateAnalysisId,
  cvController.getAnalysisStatus
);

/**
 * @route   GET /api/cv/analysis/:analysisId/feedback
 * @desc    Get AI feedback for CV analysis (optionally specific version)
 * @query   ?version=1 (optional)
 * @access  Public (in production, add authentication)
 */
router.get(
  "/:analysisId/feedback",
  validateAnalysisId,
  cvController.getAnalysisFeedback
);

/**
 * @route   POST /api/cv/analysis/:analysisId/apply-suggestion
 * @desc    Apply an improvement suggestion to analysis feedback
 * @access  Public (in production, add authentication)
 */
router.post(
  "/analysis/:analysisId/apply-suggestion",
  rateLimiter(50), // 50 applications per window
  validateAnalysisId,
  cvController.applySuggestion
);

/**
 * @route   POST /api/cv/analysis/:analysisId/reanalyze
 * @desc    Reanalyze CV with updated data (after changes in main CV service)
 * @access  Internal service (should add service authentication in production)
 */
router.post(
  "/analysis/:analysisId/reanalyze",
  rateLimiter(5), // Limited reanalysis requests
  validateAnalysisId,
  cvController.reanalyzeCV
);

/**
 * @route   GET /api/cv/:cvId/analysis-history
 * @desc    Get all analysis history for a CV (from main system)
 * @query   ?userId=uuid&page=1&limit=10
 * @access  Public (in production, add authentication)
 */
router.get(
  "/:cvId/analysis-history",
  validateCVId,
  cvController.getCVAnalysisHistory
);

/**
 * @route   GET /api/cv/user/:userId/analyses
 * @desc    Get all analysis records for a user
 * @query   ?status=completed&page=1&limit=20
 * @access  Public (in production, add authentication)
 */
router.get(
  "/user/:userId/analyses",
  validateUserId,
  cvController.getUserAnalyses
);

module.exports = router;
