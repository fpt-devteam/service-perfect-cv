const QAInteraction = require("../models/QAInteraction");
const CV = require("../models/CV");
const mistralService = require("../services/mistralService");
const logger = require("../config/logger");
const { v4: uuidv4 } = require("uuid");

class QAController {
  constructor() {
    // Bind methods to preserve 'this' context when used as route handlers
    this.askQuestion = this.askQuestion.bind(this);
    this.getConversationHistory = this.getConversationHistory.bind(this);
    this.getUserSessions = this.getUserSessions.bind(this);
    this.closeSession = this.closeSession.bind(this);
    this.getConversationAnalytics = this.getConversationAnalytics.bind(this);
  }

  // Ask a question to the AI
  async askQuestion(req, res) {
    try {
      const { userId, question, sessionId, cvId = null } = req.body;

      // Validate input
      if (!userId || !question) {
        return res.status(400).json({
          success: false,
          message: "User ID and question are required",
        });
      }

      // Generate session ID if not provided
      const activeSessionId = sessionId || uuidv4();

      // Find or create QA interaction
      let qaInteraction = await QAInteraction.findOne({
        userId,
        sessionId: activeSessionId,
        status: "active",
      });

      if (!qaInteraction) {
        qaInteraction = new QAInteraction({
          userId,
          sessionId: activeSessionId,
          cvId,
          conversation: [],
          context: {
            previousTopics: [],
            userPreferences: {},
          },
        });
      }

      // Get CV context if cvId is provided
      let cvContext = null;
      if (cvId) {
        const cv = await CV.findById(cvId).select("originalContent fileName");
        if (cv) {
          cvContext = `CV Context: ${
            cv.fileName || "Uploaded CV"
          }\n${cv.originalContent.substring(0, 500)}...`;
          qaInteraction.cvId = cvId;
        }
      }

      // Add user question to conversation
      qaInteraction.conversation.push({
        role: "user",
        message: question,
        timestamp: new Date(),
        metadata: {
          cvId: cvId,
        },
      });

      logger.info("User asked question", {
        userId,
        sessionId: activeSessionId,
        questionLength: question.length,
        cvId,
      });

      // Get AI response
      const startTime = Date.now();

      const aiResponse = await mistralService.chatQA(
        question,
        qaInteraction.conversation.slice(-10), // Last 10 messages for context
        cvContext
      );

      const responseTime = Date.now() - startTime;

      // Add AI response to conversation
      qaInteraction.conversation.push({
        role: "assistant",
        message: aiResponse.response,
        timestamp: new Date(),
        metadata: {
          responseTime,
          usage: aiResponse.usage,
        },
      });

      // Update context and topics
      await this.updateConversationContext(
        qaInteraction,
        question,
        aiResponse.response
      );

      // Save interaction
      await qaInteraction.save();

      logger.info("AI response generated", {
        userId,
        sessionId: activeSessionId,
        responseTime,
        responseLength: aiResponse.response.length,
      });

      res.json({
        success: true,
        data: {
          sessionId: activeSessionId,
          question,
          answer: aiResponse.response,
          responseTime,
          conversationLength: qaInteraction.conversation.length,
          timestamp: new Date(),
        },
      });
    } catch (error) {
      logger.error("Q&A request failed:", error);
      res.status(500).json({
        success: false,
        message: "Failed to process question",
        error:
          process.env.NODE_ENV === "development"
            ? error.message
            : "Internal server error",
      });
    }
  }

  // Get conversation history
  async getConversationHistory(req, res) {
    try {
      const { sessionId } = req.params;
      const { limit = 50 } = req.query;

      const qaInteraction = await QAInteraction.findOne({ sessionId }).populate(
        "cvId",
        "fileName fileType createdAt"
      );

      if (!qaInteraction) {
        return res.status(404).json({
          success: false,
          message: "Conversation session not found",
        });
      }

      // Get latest messages
      const conversation = qaInteraction.conversation
        .slice(-parseInt(limit))
        .map((msg) => ({
          role: msg.role,
          message: msg.message,
          timestamp: msg.timestamp,
          responseTime: msg.metadata?.responseTime,
        }));

      res.json({
        success: true,
        data: {
          sessionId,
          userId: qaInteraction.userId,
          cvContext: qaInteraction.cvId
            ? {
                cvId: qaInteraction.cvId._id,
                fileName: qaInteraction.cvId.fileName,
                fileType: qaInteraction.cvId.fileType,
              }
            : null,
          conversation,
          totalMessages: qaInteraction.totalMessages,
          lastActivity: qaInteraction.lastActivity,
          status: qaInteraction.status,
          context: qaInteraction.context,
        },
      });
    } catch (error) {
      logger.error("Failed to get conversation history:", error);
      res.status(500).json({
        success: false,
        message: "Failed to retrieve conversation history",
        error:
          process.env.NODE_ENV === "development"
            ? error.message
            : "Internal server error",
      });
    }
  }

  // Get user's active sessions
  async getUserSessions(req, res) {
    try {
      const { userId } = req.params;
      const { status = "active", page = 1, limit = 10 } = req.query;

      const skip = (page - 1) * limit;

      const sessions = await QAInteraction.find({
        userId,
        status,
      })
        .sort({ lastActivity: -1 })
        .skip(skip)
        .limit(parseInt(limit))
        .populate("cvId", "fileName fileType")
        .select(
          "sessionId cvId totalMessages lastActivity context.previousTopics conversation createdAt"
        );

      const total = await QAInteraction.countDocuments({ userId, status });

      const sessionsWithSummary = sessions.map((session) => ({
        sessionId: session.sessionId,
        cvContext: session.cvId
          ? {
              cvId: session.cvId._id,
              fileName: session.cvId.fileName,
              fileType: session.cvId.fileType,
            }
          : null,
        totalMessages: session.totalMessages,
        lastActivity: session.lastActivity,
        createdAt: session.createdAt,
        topics: session.context?.previousTopics?.slice(0, 3) || [], // First 3 topics
        lastMessage:
          session.conversation && session.conversation.length > 0
            ? session.conversation[
                session.conversation.length - 1
              ].message.substring(0, 100) + "..."
            : null,
      }));

      res.json({
        success: true,
        data: {
          sessions: sessionsWithSummary,
          pagination: {
            page: parseInt(page),
            limit: parseInt(limit),
            total,
            totalPages: Math.ceil(total / limit),
          },
        },
      });
    } catch (error) {
      logger.error("Failed to get user sessions:", error);
      res.status(500).json({
        success: false,
        message: "Failed to retrieve user sessions",
        error:
          process.env.NODE_ENV === "development"
            ? error.message
            : "Internal server error",
      });
    }
  }

  // Close conversation session
  async closeSession(req, res) {
    try {
      const { sessionId } = req.params;

      const qaInteraction = await QAInteraction.findOneAndUpdate(
        { sessionId },
        {
          status: "closed",
          lastActivity: new Date(),
        },
        { new: true }
      );

      if (!qaInteraction) {
        return res.status(404).json({
          success: false,
          message: "Conversation session not found",
        });
      }

      logger.info("Conversation session closed", {
        sessionId,
        userId: qaInteraction.userId,
        totalMessages: qaInteraction.totalMessages,
      });

      res.json({
        success: true,
        message: "Conversation session closed successfully",
        data: {
          sessionId,
          status: "closed",
          totalMessages: qaInteraction.totalMessages,
          duration: qaInteraction.lastActivity - qaInteraction.createdAt,
        },
      });
    } catch (error) {
      logger.error("Failed to close session:", error);
      res.status(500).json({
        success: false,
        message: "Failed to close conversation session",
        error:
          process.env.NODE_ENV === "development"
            ? error.message
            : "Internal server error",
      });
    }
  }

  // Get conversation analytics
  async getConversationAnalytics(req, res) {
    try {
      const { userId } = req.params;
      const { period = "30d" } = req.query;

      // Calculate date range
      const now = new Date();
      let startDate;

      switch (period) {
        case "7d":
          startDate = new Date(now.getTime() - 7 * 24 * 60 * 60 * 1000);
          break;
        case "30d":
          startDate = new Date(now.getTime() - 30 * 24 * 60 * 60 * 1000);
          break;
        case "90d":
          startDate = new Date(now.getTime() - 90 * 24 * 60 * 60 * 1000);
          break;
        default:
          startDate = new Date(now.getTime() - 30 * 24 * 60 * 60 * 1000);
      }

      // Get analytics data
      const [totalSessions, activeSessions, totalMessages, avgResponseTime] =
        await Promise.all([
          QAInteraction.countDocuments({
            userId,
            createdAt: { $gte: startDate },
          }),
          QAInteraction.countDocuments({
            userId,
            status: "active",
            lastActivity: { $gte: startDate },
          }),
          QAInteraction.aggregate([
            {
              $match: {
                userId,
                createdAt: { $gte: startDate },
              },
            },
            {
              $group: {
                _id: null,
                totalMessages: { $sum: "$totalMessages" },
              },
            },
          ]),
          QAInteraction.aggregate([
            {
              $match: {
                userId,
                createdAt: { $gte: startDate },
              },
            },
            {
              $group: {
                _id: null,
                avgResponseTime: { $avg: "$avgResponseTime" },
              },
            },
          ]),
        ]);

      // Get most common topics
      const topicAnalysis = await QAInteraction.aggregate([
        {
          $match: {
            userId,
            createdAt: { $gte: startDate },
          },
        },
        {
          $unwind: "$context.previousTopics",
        },
        {
          $group: {
            _id: "$context.previousTopics",
            count: { $sum: 1 },
          },
        },
        {
          $sort: { count: -1 },
        },
        {
          $limit: 10,
        },
      ]);

      res.json({
        success: true,
        data: {
          period,
          summary: {
            totalSessions,
            activeSessions,
            totalMessages: totalMessages[0]?.totalMessages || 0,
            avgResponseTime: avgResponseTime[0]?.avgResponseTime || 0,
            avgMessagesPerSession:
              totalSessions > 0
                ? (totalMessages[0]?.totalMessages || 0) / totalSessions
                : 0,
          },
          topTopics: topicAnalysis.map((topic) => ({
            topic: topic._id,
            count: topic.count,
          })),
        },
      });
    } catch (error) {
      logger.error("Failed to get conversation analytics:", error);
      res.status(500).json({
        success: false,
        message: "Failed to retrieve conversation analytics",
        error:
          process.env.NODE_ENV === "development"
            ? error.message
            : "Internal server error",
      });
    }
  }

  // Helper method to update conversation context
  async updateConversationContext(qaInteraction, question, response) {
    try {
      // Extract topics from question and response
      const topics = this.extractTopics(question + " " + response);

      // Update previous topics (keep unique, max 10)
      const updatedTopics = [
        ...new Set([...qaInteraction.context.previousTopics, ...topics]),
      ];
      qaInteraction.context.previousTopics = updatedTopics.slice(-10);

      // Update average response time
      const responseTimes = qaInteraction.conversation
        .filter((msg) => msg.role === "assistant" && msg.metadata?.responseTime)
        .map((msg) => msg.metadata.responseTime);

      if (responseTimes.length > 0) {
        qaInteraction.avgResponseTime =
          responseTimes.reduce((a, b) => a + b, 0) / responseTimes.length;
      }
    } catch (error) {
      logger.error("Failed to update conversation context:", error);
    }
  }

  // Helper method to extract topics from text
  extractTopics(text) {
    const keywords = [
      "resume",
      "cv",
      "experience",
      "skills",
      "education",
      "objective",
      "employment",
      "career",
      "job",
      "interview",
      "application",
      "formatting",
      "design",
      "layout",
      "content",
      "writing",
      "industry",
      "professional",
      "qualification",
      "achievement",
    ];

    const topics = [];
    const lowerText = text.toLowerCase();

    keywords.forEach((keyword) => {
      if (lowerText.includes(keyword)) {
        topics.push(keyword);
      }
    });

    return topics;
  }
}

module.exports = new QAController();
