const CVAnalysis = require("../models/CV");
const AIFeedback = require("../models/AIFeedback");
const {
  addCVAnalysisJob,
  addReanalysisJob,
  getJobStatus,
} = require("../queues/cvAnalysisQueue");
const logger = require("../config/logger");

// Helper function to calculate estimated processing time
const calculateEstimatedTime = (jobStatus) => {
  if (!jobStatus || jobStatus.finishedOn) return null;

  const progress = jobStatus.progress || 0;
  if (progress === 0) return "3-5 minutes";
  if (progress < 50) return "2-3 minutes";
  if (progress < 80) return "1-2 minutes";
  return "< 1 minute";
};

class CVController {
  // Submit structured CV data for AI analysis (called by main CV service)
  async submitCVForAnalysis(req, res) {
    try {
      const {
        cvId, // UUID from main CV system
        userId, // UUID from main CV system
        cvData, // Structured CV data matching your schema
        userPreferences = {},
        requestMetadata = {},
      } = req.body;

      // Validate required fields
      if (!cvId || !userId || !cvData) {
        return res.status(400).json({
          success: false,
          message: "CV ID, User ID, and CV data are required",
        });
      }

      // Check if analysis already exists for this CV
      const existingAnalysis = await CVAnalysis.findOne({ cvId, userId });
      if (existingAnalysis) {
        // Check job status if there's a processing job ID
        let jobStatus = null;
        if (existingAnalysis.processingJobId) {
          try {
            jobStatus = await getJobStatus(existingAnalysis.processingJobId);
            logger.debug("Job status check for existing analysis", {
              analysisId: existingAnalysis._id,
              jobId: existingAnalysis.processingJobId,
              analysisStatus: existingAnalysis.status,
              jobStatus: jobStatus?.status,
              jobFailedReason: jobStatus?.failedReason,
              fullJobStatus: jobStatus,
            });
          } catch (error) {
            logger.warn("Failed to get job status", {
              jobId: existingAnalysis.processingJobId,
              error: error.message,
            });
          }
        }

        // If analysis is processing or pending with an active job, return conflict
        if (
          existingAnalysis.status === "processing" ||
          (existingAnalysis.status === "pending" &&
            jobStatus &&
            (jobStatus.status === "active" ||
              jobStatus.status === "waiting" ||
              jobStatus.status === "delayed"))
        ) {
          logger.debug("Returning conflict - analysis in progress", {
            analysisStatus: existingAnalysis.status,
            jobStatus: jobStatus?.status,
            shouldReturnConflict: true,
          });

          return res.status(409).json({
            success: false,
            message: "CV analysis already in progress",
            data: {
              analysisId: existingAnalysis._id,
              jobId: existingAnalysis.processingJobId,
              status: existingAnalysis.status,
            },
          });
        }

        // If previous analysis failed OR job failed, we can resubmit
        if (
          existingAnalysis.status === "failed" ||
          (existingAnalysis.status === "pending" &&
            jobStatus &&
            jobStatus.status === "failed")
        ) {
          logger.info("Previous CV analysis failed, allowing resubmission", {
            analysisId: existingAnalysis._id,
            cvId,
            userId,
            previousStatus: existingAnalysis.status,
            jobStatus: jobStatus?.status,
            jobFailedReason: jobStatus?.failedReason,
            shouldAllowResubmission: true,
          });

          // Update the existing record instead of creating a new one
          existingAnalysis.cvData = cvData;
          existingAnalysis.userPreferences = {
            targetIndustry: userPreferences.targetIndustry,
            targetRole: userPreferences.targetRole,
            experienceLevel: userPreferences.experienceLevel,
            focusAreas: userPreferences.focusAreas || [],
            urgent: userPreferences.urgent || false,
          };
          existingAnalysis.requestMetadata = {
            source: requestMetadata.source || "main-cv-service",
            version: requestMetadata.version,
            submittedAt: new Date(),
          };
          existingAnalysis.status = "pending";
          existingAnalysis.processingJobId = null;

          await existingAnalysis.save();

          logger.info("CV resubmitted for AI analysis after failure", {
            analysisId: existingAnalysis._id,
            cvId,
            userId,
          });

          // Add to processing queue
          const job = await addCVAnalysisJob({
            cvAnalysisId: existingAnalysis._id,
            cvId,
            userId,
            userPreferences: existingAnalysis.userPreferences,
            priority: userPreferences.urgent ? 10 : 0,
          });

          // Update analysis record with job ID
          existingAnalysis.processingJobId = job.id;
          await existingAnalysis.save();

          return res.status(201).json({
            success: true,
            message: "CV resubmitted successfully for AI analysis",
            data: {
              analysisId: existingAnalysis._id,
              cvId,
              jobId: job.id,
              status: existingAnalysis.status,
              estimatedProcessingTime: "2-5 minutes",
              queuePosition: (await job.opts.delay) ? "delayed" : "active",
              isResubmission: true,
            },
          });
        }

        // If analysis is completed, suggest using reanalysis endpoint
        if (existingAnalysis.status === "completed") {
          return res.status(409).json({
            success: false,
            message:
              "CV analysis already completed. Use reanalysis endpoint for updated analysis.",
            data: {
              analysisId: existingAnalysis._id,
              status: existingAnalysis.status,
              completedAt: existingAnalysis.updatedAt,
              suggestion:
                "Use POST /api/cv/analyze/:analysisId/reanalyze to get updated analysis",
            },
          });
        }
      }

      // Create CV analysis record
      const cvAnalysis = new CVAnalysis({
        cvId,
        userId,
        cvData,
        userPreferences: {
          targetIndustry: userPreferences.targetIndustry,
          targetRole: userPreferences.targetRole,
          experienceLevel: userPreferences.experienceLevel,
          focusAreas: userPreferences.focusAreas || [],
          urgent: userPreferences.urgent || false,
        },
        requestMetadata: {
          source: requestMetadata.source || "main-cv-service",
          version: requestMetadata.version,
          submittedAt: new Date(),
        },
        status: "pending",
      });

      await cvAnalysis.save();

      logger.info("CV submitted for AI analysis", {
        analysisId: cvAnalysis._id,
        cvId,
        userId,
        hasContact: !!cvData.contact,
        hasExperience: !!(cvData.experience && cvData.experience.length > 0),
        hasEducation: !!(cvData.education && cvData.education.length > 0),
        hasProjects: !!(cvData.projects && cvData.projects.length > 0),
        hasSkills: !!(cvData.skills && cvData.skills.length > 0),
        targetIndustry: userPreferences.targetIndustry,
        targetRole: userPreferences.targetRole,
      });

      // Add to processing queue
      const job = await addCVAnalysisJob({
        cvAnalysisId: cvAnalysis._id,
        cvId,
        userId,
        userPreferences: cvAnalysis.userPreferences,
        priority: userPreferences.urgent ? 10 : 0,
      });

      // Update analysis record with job ID
      cvAnalysis.processingJobId = job.id;
      await cvAnalysis.save();

      res.status(201).json({
        success: true,
        message: "CV submitted successfully for AI analysis",
        data: {
          analysisId: cvAnalysis._id,
          cvId,
          jobId: job.id,
          status: cvAnalysis.status,
          estimatedProcessingTime: "2-5 minutes",
          queuePosition: (await job.opts.delay) ? "delayed" : "active",
        },
      });
    } catch (error) {
      logger.error("CV analysis submission failed:", error);
      res.status(500).json({
        success: false,
        message: "Failed to submit CV for analysis",
        error:
          process.env.NODE_ENV === "development"
            ? error.message
            : "Internal server error",
      });
    }
  }

  // Get CV analysis processing status
  async getAnalysisStatus(req, res) {
    try {
      const { analysisId } = req.params;

      const analysis = await CVAnalysis.findById(analysisId);
      if (!analysis) {
        return res.status(404).json({
          success: false,
          message: "CV analysis not found",
        });
      }

      let jobStatus = null;
      if (analysis.processingJobId) {
        jobStatus = await getJobStatus(analysis.processingJobId);
      }

      res.json({
        success: true,
        data: {
          analysisId: analysis._id,
          cvId: analysis.cvId,
          userId: analysis.userId,
          status: analysis.status,
          createdAt: analysis.createdAt,
          updatedAt: analysis.updatedAt,
          userPreferences: analysis.userPreferences,
          jobStatus: jobStatus,
          processingProgress: jobStatus ? jobStatus.progress : null,
          estimatedTimeRemaining: calculateEstimatedTime(jobStatus),
        },
      });
    } catch (error) {
      logger.error("Failed to get analysis status:", error);
      res.status(500).json({
        success: false,
        message: "Failed to retrieve analysis status",
        error:
          process.env.NODE_ENV === "development"
            ? error.message
            : "Internal server error",
      });
    }
  }

  // Get AI feedback for CV analysis
  async getAnalysisFeedback(req, res) {
    try {
      const { analysisId } = req.params;
      const { version } = req.query;

      const analysis = await CVAnalysis.findById(analysisId);
      if (!analysis) {
        return res.status(404).json({
          success: false,
          message: "CV analysis not found",
        });
      }

      // Build query for feedback
      let feedbackQuery = { cvAnalysisId: analysisId, cvId: analysis.cvId };
      if (version) {
        feedbackQuery.version = parseInt(version);
      }

      const feedback = await AIFeedback.findOne(feedbackQuery)
        .sort({ createdAt: -1 })
        .populate("cvAnalysisId", "cvId userId createdAt userPreferences");

      if (!feedback) {
        return res.status(404).json({
          success: false,
          message: version
            ? `Feedback version ${version} not found`
            : "No feedback available yet. Analysis may still be in progress.",
        });
      }

      res.json({
        success: true,
        data: {
          feedbackId: feedback._id,
          analysisId: feedback.cvAnalysisId._id,
          cvId: feedback.cvId,
          userId: feedback.userId,
          version: feedback.version,
          analysis: {
            overallScore: feedback.feedback.overallScore,
            sectionAnalysis: feedback.feedback.sectionAnalysis,
            improvementSuggestions: feedback.feedback.improvementSuggestions,
            industrySpecificAdvice: feedback.feedback.industrySpecificAdvice,
            atsCompatibility: feedback.feedback.atsCompatibility,
          },
          metadata: {
            processingTime: feedback.processingTime,
            modelInfo: feedback.modelInfo,
            createdAt: feedback.createdAt,
          },
          userPreferences: feedback.cvAnalysisId.userPreferences,
        },
      });
    } catch (error) {
      logger.error("Failed to get analysis feedback:", error);
      res.status(500).json({
        success: false,
        message: "Failed to retrieve analysis feedback",
        error:
          process.env.NODE_ENV === "development"
            ? error.message
            : "Internal server error",
      });
    }
  }

  // Apply improvement suggestion
  async applySuggestion(req, res) {
    try {
      const { analysisId } = req.params;
      const { suggestionId, appliedBy } = req.body;

      if (!suggestionId) {
        return res.status(400).json({
          success: false,
          message: "Suggestion ID is required",
        });
      }

      const feedback = await AIFeedback.findOne({
        cvAnalysisId: analysisId,
      }).sort({
        createdAt: -1,
      });

      if (!feedback) {
        return res.status(404).json({
          success: false,
          message: "No feedback found for this analysis",
        });
      }

      // Find and update the suggestion
      const suggestion = feedback.feedback.improvementSuggestions.find(
        (s) => s.id === suggestionId
      );

      if (!suggestion) {
        return res.status(404).json({
          success: false,
          message: "Suggestion not found",
        });
      }

      if (suggestion.appliedAt) {
        return res.status(409).json({
          success: false,
          message: "Suggestion has already been applied",
          data: {
            appliedAt: suggestion.appliedAt,
            appliedBy: suggestion.appliedBy,
          },
        });
      }

      // Mark suggestion as applied
      suggestion.appliedAt = new Date();
      suggestion.appliedBy = appliedBy || "user";

      await feedback.save();

      logger.info("Improvement suggestion applied", {
        analysisId,
        suggestionId,
        section: suggestion.section,
        priority: suggestion.priority,
        appliedBy: suggestion.appliedBy,
      });

      res.json({
        success: true,
        message: "Suggestion applied successfully",
        data: {
          suggestionId: suggestion.id,
          section: suggestion.section,
          suggestion: suggestion.suggestion,
          appliedAt: suggestion.appliedAt,
          appliedBy: suggestion.appliedBy,
        },
      });
    } catch (error) {
      logger.error("Failed to apply suggestion:", error);
      res.status(500).json({
        success: false,
        message: "Failed to apply suggestion",
        error:
          process.env.NODE_ENV === "development"
            ? error.message
            : "Internal server error",
      });
    }
  }

  // Request reanalysis of CV (after changes made in main system)
  async reanalyzeCV(req, res) {
    try {
      const { analysisId } = req.params;
      const { updatedCvData, userPreferences = {}, reason } = req.body;

      const analysis = await CVAnalysis.findById(analysisId);
      if (!analysis) {
        return res.status(404).json({
          success: false,
          message: "CV analysis not found",
        });
      }

      // Update CV data if provided
      if (updatedCvData) {
        analysis.cvData = updatedCvData;
      }

      // Update user preferences if provided
      if (Object.keys(userPreferences).length > 0) {
        analysis.userPreferences = {
          ...analysis.userPreferences,
          ...userPreferences,
        };
      }

      // Reset status for reanalysis
      analysis.status = "pending";
      analysis.processingJobId = null;

      await analysis.save();

      logger.info("CV reanalysis requested", {
        analysisId,
        cvId: analysis.cvId,
        userId: analysis.userId,
        reason: reason || "manual_request",
        hasUpdatedData: !!updatedCvData,
      });

      // Add reanalysis job to queue
      const job = await addReanalysisJob({
        cvAnalysisId: analysis._id,
        cvId: analysis.cvId,
        userId: analysis.userId,
        userPreferences: analysis.userPreferences,
        reason: reason || "manual_request",
        priority: userPreferences.urgent ? 10 : 5, // Higher priority for reanalysis
      });

      // Update with new job ID
      analysis.processingJobId = job.id;
      await analysis.save();

      res.json({
        success: true,
        message: "CV reanalysis started successfully",
        data: {
          analysisId: analysis._id,
          cvId: analysis.cvId,
          jobId: job.id,
          status: analysis.status,
          reason: reason || "manual_request",
          estimatedProcessingTime: "2-5 minutes",
        },
      });
    } catch (error) {
      logger.error("CV reanalysis failed:", error);
      res.status(500).json({
        success: false,
        message: "Failed to start CV reanalysis",
        error:
          process.env.NODE_ENV === "development"
            ? error.message
            : "Internal server error",
      });
    }
  }

  // Get analysis history for a CV (all versions)
  async getCVAnalysisHistory(req, res) {
    try {
      const { cvId } = req.params;
      const { userId } = req.query;
      const page = parseInt(req.query.page) || 1;
      const limit = parseInt(req.query.limit) || 10;
      const skip = (page - 1) * limit;

      let query = { cvId };
      if (userId) {
        query.userId = userId;
      }

      const analyses = await CVAnalysis.find(query)
        .sort({ createdAt: -1 })
        .skip(skip)
        .limit(limit)
        .select(
          "_id userId status userPreferences requestMetadata createdAt updatedAt"
        );

      const total = await CVAnalysis.countDocuments(query);

      // Get feedback for each analysis
      const analysisIds = analyses.map((a) => a._id);
      const feedbacks = await AIFeedback.find({
        cvAnalysisId: { $in: analysisIds },
      }).select("cvAnalysisId feedback.overallScore version createdAt");

      // Create a map for quick lookup
      const feedbackMap = {};
      feedbacks.forEach((fb) => {
        if (!feedbackMap[fb.cvAnalysisId]) {
          feedbackMap[fb.cvAnalysisId] = [];
        }
        feedbackMap[fb.cvAnalysisId].push({
          overallScore: fb.feedback.overallScore,
          version: fb.version,
          createdAt: fb.createdAt,
        });
      });

      const history = analyses.map((analysis) => ({
        analysisId: analysis._id,
        userId: analysis.userId,
        status: analysis.status,
        userPreferences: analysis.userPreferences,
        requestMetadata: analysis.requestMetadata,
        createdAt: analysis.createdAt,
        updatedAt: analysis.updatedAt,
        feedback: feedbackMap[analysis._id] || [],
      }));

      res.json({
        success: true,
        data: {
          cvId,
          history,
          pagination: {
            page,
            limit,
            total,
            pages: Math.ceil(total / limit),
            hasNext: page < Math.ceil(total / limit),
            hasPrev: page > 1,
          },
        },
      });
    } catch (error) {
      logger.error("Failed to get CV analysis history:", error);
      res.status(500).json({
        success: false,
        message: "Failed to retrieve CV analysis history",
        error:
          process.env.NODE_ENV === "development"
            ? error.message
            : "Internal server error",
      });
    }
  }

  // Get all analyses for a user
  async getUserAnalyses(req, res) {
    try {
      const { userId } = req.params;
      const page = parseInt(req.query.page) || 1;
      const limit = parseInt(req.query.limit) || 20;
      const status = req.query.status;
      const skip = (page - 1) * limit;

      let query = { userId };
      if (status) {
        query.status = status;
      }

      const analyses = await CVAnalysis.find(query)
        .sort({ createdAt: -1 })
        .skip(skip)
        .limit(limit)
        .select("_id cvId status userPreferences requestMetadata createdAt");

      const total = await CVAnalysis.countDocuments(query);

      // Get latest feedback for each analysis
      const analysisIds = analyses.map((a) => a._id);
      const latestFeedbacks = await AIFeedback.aggregate([
        { $match: { cvAnalysisId: { $in: analysisIds } } },
        { $sort: { cvAnalysisId: 1, createdAt: -1 } },
        {
          $group: {
            _id: "$cvAnalysisId",
            overallScore: { $first: "$feedback.overallScore" },
            version: { $first: "$version" },
            createdAt: { $first: "$createdAt" },
          },
        },
      ]);

      const feedbackMap = {};
      latestFeedbacks.forEach((fb) => {
        feedbackMap[fb._id] = {
          overallScore: fb.overallScore,
          version: fb.version,
          createdAt: fb.createdAt,
        };
      });

      const userAnalyses = analyses.map((analysis) => ({
        analysisId: analysis._id,
        cvId: analysis.cvId,
        status: analysis.status,
        userPreferences: analysis.userPreferences,
        requestMetadata: analysis.requestMetadata,
        createdAt: analysis.createdAt,
        latestFeedback: feedbackMap[analysis._id] || null,
      }));

      res.json({
        success: true,
        data: {
          userId,
          analyses: userAnalyses,
          pagination: {
            page,
            limit,
            total,
            pages: Math.ceil(total / limit),
            hasNext: page < Math.ceil(total / limit),
            hasPrev: page > 1,
          },
          summary: {
            totalAnalyses: total,
            completedAnalyses: analyses.filter((a) => a.status === "completed")
              .length,
            pendingAnalyses: analyses.filter((a) => a.status === "pending")
              .length,
            processingAnalyses: analyses.filter(
              (a) => a.status === "processing"
            ).length,
          },
        },
      });
    } catch (error) {
      logger.error("Failed to get user analyses:", error);
      res.status(500).json({
        success: false,
        message: "Failed to retrieve user analyses",
        error:
          process.env.NODE_ENV === "development"
            ? error.message
            : "Internal server error",
      });
    }
  }
}

module.exports = new CVController();
