const express = require("express");
const qaController = require("../controllers/qaController");
const {
  validateQuestion,
  validateSessionId,
  validateUserId,
} = require("../middleware/validation");
const { rateLimiter } = require("../middleware/rateLimiter");

const router = express.Router();

// Q&A Chat Routes

/**
 * @route   POST /api/qa/ask
 * @desc    Ask a question to the AI assistant
 * @access  Public (in production, add authentication)
 */
router.post(
  "/ask",
  rateLimiter(20), // 20 questions per window
  validateQuestion,
  qaController.askQuestion
);

/**
 * @route   GET /api/qa/session/:sessionId
 * @desc    Get conversation history for a session
 * @query   ?limit=50
 * @access  Public (in production, add authentication)
 */
router.get(
  "/session/:sessionId",
  validateSessionId,
  qaController.getConversationHistory
);

/**
 * @route   POST /api/qa/session/:sessionId/close
 * @desc    Close a conversation session
 * @access  Public (in production, add authentication)
 */
router.post(
  "/session/:sessionId/close",
  validateSessionId,
  qaController.closeSession
);

/**
 * @route   GET /api/qa/user/:userId/sessions
 * @desc    Get user's conversation sessions
 * @query   ?status=active&page=1&limit=10
 * @access  Public (in production, add authentication)
 */
router.get(
  "/user/:userId/sessions",
  validateUserId,
  qaController.getUserSessions
);

/**
 * @route   GET /api/qa/user/:userId/analytics
 * @desc    Get conversation analytics for user
 * @query   ?period=30d
 * @access  Public (in production, add authentication)
 */
router.get(
  "/user/:userId/analytics",
  validateUserId,
  qaController.getConversationAnalytics
);

module.exports = router;
