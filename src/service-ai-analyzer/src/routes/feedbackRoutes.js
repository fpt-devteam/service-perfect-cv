const express = require("express");
const feedbackController = require("../controllers/feedbackController");
const { validateUserId } = require("../middleware/validation");
const { rateLimiter } = require("../middleware/rateLimiter");

const router = express.Router();

// Feedback & Progress Routes

/**
 * @route   GET /api/feedback/user/:userId/history
 * @desc    Get user's feedback history
 * @query   ?page=1&limit=20
 * @access  Public (in production, add authentication)
 */
router.get(
  "/user/:userId/history",
  validateUserId,
  feedbackController.getFeedbackHistory
);

/**
 * @route   GET /api/feedback/user/:userId/dashboard
 * @desc    Get user's progress dashboard
 * @access  Public (in production, add authentication)
 */
router.get(
  "/user/:userId/dashboard",
  validateUserId,
  feedbackController.getProgressDashboard
);

/**
 * @route   POST /api/feedback/user/:userId/apply-suggestion
 * @desc    Apply suggestion and update progress
 * @access  Public (in production, add authentication)
 */
router.post(
  "/user/:userId/apply-suggestion",
  rateLimiter(100), // 100 suggestion applications per window
  validateUserId,
  feedbackController.applySuggestion
);

/**
 * @route   GET /api/feedback/user/:userId/analytics
 * @desc    Get improvement analytics
 * @query   ?period=30d
 * @access  Public (in production, add authentication)
 */
router.get(
  "/user/:userId/analytics",
  validateUserId,
  feedbackController.getImprovementAnalytics
);

/**
 * @route   GET /api/feedback/user/:userId/compare
 * @desc    Compare CV versions
 * @query   ?cvId=xxx&fromVersion=1&toVersion=2
 * @access  Public (in production, add authentication)
 */
router.get(
  "/user/:userId/compare",
  validateUserId,
  feedbackController.compareVersions
);

module.exports = router;
