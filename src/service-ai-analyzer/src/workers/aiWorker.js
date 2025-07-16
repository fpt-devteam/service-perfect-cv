require("dotenv").config();
const { cvAnalysisQueue, JOB_TYPES } = require("../queues/cvAnalysisQueue");
const mistralService = require("../services/mistralService");
const connectDB = require("../config/database");
const logger = require("../config/logger");
const CVAnalysis = require("../models/CV");
const AIFeedback = require("../models/AIFeedback");
const FeedbackHistory = require("../models/FeedbackHistory");
const { v4: uuidv4 } = require("uuid");
const mongoose = require("mongoose");

const initializeWorker = async () => {
  try {
    await connectDB();

    if (mongoose.connection.readyState !== 1) {
      await new Promise((resolve, reject) => {
        mongoose.connection.once("connected", resolve);
        mongoose.connection.once("error", reject);
        setTimeout(
          () => reject(new Error("Database connection timeout")),
          10000
        );
      });
    }

    logger.info("AI Worker initialized with database connection ready");
  } catch (error) {
    logger.error("Failed to initialize AI Worker:", error);
    process.exit(1);
  }
};

const findCVAnalysisWithRetry = async (
  cvAnalysisId,
  maxRetries = 3,
  delay = 1000
) => {
  for (let attempt = 1; attempt <= maxRetries; attempt++) {
    try {
      logger.debug(
        `Attempting to find CV analysis record (attempt ${attempt}/${maxRetries})`,
        {
          cvAnalysisId,
          dbState: mongoose.connection.readyState,
        }
      );

      const cvAnalysis = await CVAnalysis.findById(cvAnalysisId);

      if (cvAnalysis) {
        logger.debug("CV analysis record found successfully", {
          cvAnalysisId,
          status: cvAnalysis.status,
          attempt,
        });
        return cvAnalysis;
      }

      if (attempt < maxRetries) {
        logger.warn(
          `CV analysis record not found on attempt ${attempt}, retrying in ${delay}ms`,
          {
            cvAnalysisId,
          }
        );
        await new Promise((resolve) => setTimeout(resolve, delay));
        delay *= 2;
      }
    } catch (error) {
      logger.error(`Error finding CV analysis record on attempt ${attempt}:`, {
        cvAnalysisId,
        error: error.message,
        attempt,
      });

      if (attempt === maxRetries) {
        throw error;
      }

      await new Promise((resolve) => setTimeout(resolve, delay));
      delay *= 2;
    }
  }

  return null;
};

initializeWorker();

cvAnalysisQueue.process(JOB_TYPES.ANALYZE_CV, 2, async (job) => {
  const { cvAnalysisId, cvId, userId, userPreferences = {} } = job.data;

  try {
    logger.info("Starting CV analysis job", {
      jobId: job.id,
      cvAnalysisId,
      cvId,
      userId,
      targetIndustry: userPreferences.targetIndustry,
      targetRole: userPreferences.targetRole,
      dbState: mongoose.connection.readyState,
    });

    await job.progress(10);

    const cvAnalysis = await findCVAnalysisWithRetry(cvAnalysisId);
    if (!cvAnalysis) {
      const rawDoc = await mongoose.connection.db
        .collection(CVAnalysis.collection.name)
        .findOne({
          _id: mongoose.Types.ObjectId(cvAnalysisId),
        });

      if (!rawDoc) {
        throw new Error(
          `CV analysis record not found: ${cvAnalysisId} (verified with raw query)`
        );
      } else {
        logger.warn(
          "CV analysis found with raw query but not with Mongoose - possible model/schema issue",
          {
            cvAnalysisId,
            rawDoc: rawDoc._id,
            collectionName: CVAnalysis.collection.name,
          }
        );
        throw new Error(
          `CV analysis record not accessible through Mongoose: ${cvAnalysisId}`
        );
      }
    }

    cvAnalysis.status = "processing";
    await cvAnalysis.save();

    await job.progress(20);

    logger.info("Sending structured CV data to Mistral AI for analysis", {
      cvAnalysisId,
      cvId,
      hasContact: !!cvAnalysis.cvData.contact,
      hasExperience: !!(
        cvAnalysis.cvData.experience && cvAnalysis.cvData.experience.length > 0
      ),
      hasEducation: !!(
        cvAnalysis.cvData.education && cvAnalysis.cvData.education.length > 0
      ),
      hasProjects: !!(
        cvAnalysis.cvData.projects && cvAnalysis.cvData.projects.length > 0
      ),
      hasSkills: !!(
        cvAnalysis.cvData.skills && cvAnalysis.cvData.skills.length > 0
      ),
    });

    const result = await mistralService.analyzCV(
      cvAnalysis.cvData,
      userPreferences
    );

    await job.progress(60);

    const feedback = new AIFeedback({
      cvAnalysisId: cvAnalysis._id,
      cvId: cvAnalysis.cvId,
      userId: userId,
      feedback: result.analysis,
      mistralResponse: result.rawResponse,
      processingTime: result.processingTime,
      version: 1,
      modelInfo: {
        model: result.rawResponse.model,
        temperature: 0.3,
        tokensUsed: result.rawResponse.usage?.total_tokens || 0,
        requestId: result.rawResponse.id,
      },
    });

    if (feedback.feedback.improvementSuggestions) {
      feedback.feedback.improvementSuggestions =
        feedback.feedback.improvementSuggestions.map((suggestion) => ({
          ...suggestion,
          id: suggestion.id || uuidv4(),
        }));
    }

    await feedback.save();

    await job.progress(80);

    await updateFeedbackHistory(
      userId,
      cvAnalysis.cvId,
      feedback._id,
      result.analysis.overallScore
    );

    await job.progress(90);

    cvAnalysis.status = "completed";
    await cvAnalysis.save();

    await job.progress(100);

    logger.info("CV analysis completed successfully", {
      jobId: job.id,
      cvAnalysisId,
      cvId,
      userId,
      overallScore: result.analysis.overallScore,
      processingTime: result.processingTime,
      tokensUsed: result.rawResponse.usage?.total_tokens || 0,
    });

    return {
      success: true,
      cvAnalysisId: cvAnalysis._id,
      cvId: cvAnalysis.cvId,
      feedbackId: feedback._id,
      overallScore: result.analysis.overallScore,
      processingTime: result.processingTime,
      tokensUsed: result.rawResponse.usage?.total_tokens || 0,
    };
  } catch (error) {
    logger.error("CV analysis job failed", {
      jobId: job.id,
      cvAnalysisId,
      cvId,
      userId,
      error: error.message,
      stack: error.stack,
      dbState: mongoose.connection.readyState,
    });

    try {
      const cvAnalysis = await findCVAnalysisWithRetry(cvAnalysisId, 2, 500);
      if (cvAnalysis) {
        cvAnalysis.status = "failed";
        await cvAnalysis.save();
      }
    } catch (updateError) {
      logger.error("Failed to update analysis status to failed", {
        cvAnalysisId,
        error: updateError.message,
      });
    }

    throw error;
  }
});

cvAnalysisQueue.process(JOB_TYPES.REANALYZE_CV, 1, async (job) => {
  const {
    cvAnalysisId,
    cvId,
    userId,
    userPreferences = {},
    reason = "manual_request",
  } = job.data;

  try {
    logger.info("Starting CV reanalysis job", {
      jobId: job.id,
      cvAnalysisId,
      cvId,
      userId,
      reason,
    });

    await job.progress(10);

    const cvAnalysis = await findCVAnalysisWithRetry(cvAnalysisId);
    if (!cvAnalysis) {
      throw new Error(`CV analysis record not found: ${cvAnalysisId}`);
    }

    cvAnalysis.status = "processing";
    await cvAnalysis.save();

    await job.progress(20);

    const previousFeedback = await AIFeedback.findOne({
      cvAnalysisId: cvAnalysisId,
    }).sort({
      createdAt: -1,
    });
    const previousScore = previousFeedback
      ? previousFeedback.feedback.overallScore
      : 0;

    await job.progress(30);

    const result = await mistralService.analyzCV(
      cvAnalysis.cvData,
      userPreferences
    );

    await job.progress(70);

    const newVersion = previousFeedback ? previousFeedback.version + 1 : 1;
    const feedback = new AIFeedback({
      cvAnalysisId: cvAnalysis._id,
      cvId: cvAnalysis.cvId,
      userId: userId,
      feedback: result.analysis,
      mistralResponse: result.rawResponse,
      processingTime: result.processingTime,
      version: newVersion,
      modelInfo: {
        model: result.rawResponse.model,
        temperature: 0.3,
        tokensUsed: result.rawResponse.usage?.total_tokens || 0,
        requestId: result.rawResponse.id,
      },
    });

    if (feedback.feedback.improvementSuggestions) {
      feedback.feedback.improvementSuggestions =
        feedback.feedback.improvementSuggestions.map((suggestion) => ({
          ...suggestion,
          id: suggestion.id || uuidv4(),
        }));
    }

    await feedback.save();

    await job.progress(85);

    const scoreImprovement = result.analysis.overallScore - previousScore;
    const changes = [
      `Reanalysis (${reason}) - Score change: ${previousScore} → ${
        result.analysis.overallScore
      } (${scoreImprovement >= 0 ? "+" : ""}${scoreImprovement})`,
    ];

    await updateFeedbackHistory(
      userId,
      cvAnalysis.cvId,
      feedback._id,
      result.analysis.overallScore,
      newVersion,
      changes
    );

    await job.progress(95);

    cvAnalysis.status = "completed";
    await cvAnalysis.save();

    await job.progress(100);

    logger.info("CV reanalysis completed successfully", {
      jobId: job.id,
      cvAnalysisId,
      cvId,
      userId,
      previousScore,
      newScore: result.analysis.overallScore,
      improvement: result.analysis.overallScore - previousScore,
      version: newVersion,
    });

    return {
      success: true,
      cvAnalysisId: cvAnalysis._id,
      cvId: cvAnalysis.cvId,
      feedbackId: feedback._id,
      previousScore,
      newScore: result.analysis.overallScore,
      improvement: result.analysis.overallScore - previousScore,
      version: newVersion,
    };
  } catch (error) {
    logger.error("CV reanalysis job failed", {
      jobId: job.id,
      cvAnalysisId,
      cvId,
      userId,
      error: error.message,
    });

    try {
      const cvAnalysis = await findCVAnalysisWithRetry(cvAnalysisId, 2, 500);
      if (cvAnalysis) {
        cvAnalysis.status = "failed";
        await cvAnalysis.save();
      }
    } catch (updateError) {
      logger.error("Failed to update analysis status to failed", updateError);
    }

    throw error;
  }
});

cvAnalysisQueue.process(JOB_TYPES.GENERATE_SUGGESTIONS, 2, async (job) => {
  const { cvId, section, specificIssues = [] } = job.data;

  try {
    logger.info("Starting suggestion generation job", {
      jobId: job.id,
      cvId,
      section,
    });

    await job.progress(20);

    const cv = await CVAnalysis.findById(cvId);
    if (!cv) {
      throw new Error(`CV not found: ${cvId}`);
    }

    await job.progress(40);

    const suggestions = await mistralService.generateImprovementSuggestions(
      cv.cvData,
      section,
      specificIssues
    );

    await job.progress(80);

    const latestFeedback = await AIFeedback.findOne({ cvId }).sort({
      createdAt: -1,
    });

    if (latestFeedback && latestFeedback.feedback.improvementSuggestions) {
      const newSuggestions = suggestions.map((suggestion) => ({
        id: uuidv4(),
        section: section,
        priority: "medium",
        suggestion: suggestion,
        explanation: `Targeted improvement for ${section}`,
        autoApplicable: false,
        category: "generated",
      }));

      latestFeedback.feedback.improvementSuggestions.push(...newSuggestions);
      await latestFeedback.save();
    }

    await job.progress(100);

    logger.info("Suggestion generation completed successfully", {
      jobId: job.id,
      cvId,
      section,
      suggestionsCount: suggestions.length,
    });

    return {
      success: true,
      cvId,
      section,
      suggestions,
      suggestionsCount: suggestions.length,
    };
  } catch (error) {
    logger.error("Suggestion generation job failed", {
      jobId: job.id,
      cvId,
      section,
      error: error.message,
    });

    throw error;
  }
});

async function updateFeedbackHistory(
  userId,
  cvId,
  feedbackId,
  overallScore,
  version = 1,
  changes = []
) {
  try {
    let history = await FeedbackHistory.findOne({ userId });

    if (!history) {
      history = new FeedbackHistory({
        userId,
        cvVersions: [],
        progressMetrics: {
          initialScore: overallScore,
          currentScore: overallScore,
          bestScore: overallScore,
          totalImprovements: 0,
          improvementAreas: [],
          streaks: {
            currentStreak: 1,
            longestStreak: 1,
            lastActivityDate: new Date(),
          },
        },
        statistics: {
          totalCVsAnalyzed: 1,
          totalSuggestionsReceived: 0,
          totalSuggestionsApplied: 0,
          averageScoreImprovement: 0,
          timeSpentImproving: 0,
        },
      });
    }

    const cvVersionData = {
      cvId,
      feedbackId,
      version,
      overallScore,
      changes,
      timestamp: new Date(),
    };

    await history.addCVVersion(cvVersionData);

    logger.info("Feedback history updated", {
      userId,
      cvId,
      version,
      overallScore,
    });
  } catch (error) {
    logger.error("Failed to update feedback history", {
      userId,
      cvId,
      error: error.message,
    });
  }
}

process.on("SIGTERM", async () => {
  logger.info("Worker received SIGTERM, shutting down gracefully...");
  await cvAnalysisQueue.close();
  process.exit(0);
});

process.on("SIGINT", async () => {
  logger.info("Worker received SIGINT, shutting down gracefully...");
  await cvAnalysisQueue.close();
  process.exit(0);
});

logger.info("AI Worker started, processing CV analysis jobs...");
