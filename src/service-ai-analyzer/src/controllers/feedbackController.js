const FeedbackHistory = require("../models/FeedbackHistory");
const AIFeedback = require("../models/AIFeedback");
const CV = require("../models/CV");
const logger = require("../config/logger");

class FeedbackController {
  // Get user's feedback history
  async getFeedbackHistory(req, res) {
    try {
      const { userId } = req.params;
      const { page = 1, limit = 20 } = req.query;

      const history = await FeedbackHistory.findOne({ userId })
        .populate({
          path: "cvVersions.cvId",
          select: "fileName fileType createdAt",
        })
        .populate({
          path: "cvVersions.feedbackId",
          select: "feedback.overallScore feedback.sectionAnalysis createdAt",
        });

      if (!history) {
        return res.status(404).json({
          success: false,
          message: "No feedback history found for this user",
        });
      }

      // Paginate CV versions
      const skip = (page - 1) * limit;
      const paginatedVersions = history.cvVersions
        .sort((a, b) => new Date(b.timestamp) - new Date(a.timestamp))
        .slice(skip, skip + parseInt(limit));

      // Calculate progress metrics
      const progressData = this.calculateProgressMetrics(history);

      res.json({
        success: true,
        data: {
          userId,
          progressMetrics: history.progressMetrics,
          progressData,
          cvVersions: paginatedVersions.map((version) => ({
            cvId: version.cvId._id,
            cvFileName: version.cvId.fileName,
            version: version.version,
            overallScore: version.overallScore,
            changes: version.changes,
            appliedSuggestions: version.appliedSuggestions,
            timestamp: version.timestamp,
            sectionScores: version.feedbackId
              ? {
                  careerObjective:
                    version.feedbackId.feedback.sectionAnalysis.careerObjective
                      ?.score,
                  skills:
                    version.feedbackId.feedback.sectionAnalysis.skills?.score,
                  experience:
                    version.feedbackId.feedback.sectionAnalysis.experience
                      ?.score,
                  education:
                    version.feedbackId.feedback.sectionAnalysis.education
                      ?.score,
                  projects:
                    version.feedbackId.feedback.sectionAnalysis.projects?.score,
                  formatting:
                    version.feedbackId.feedback.sectionAnalysis.formatting
                      ?.score,
                }
              : null,
          })),
          achievements: history.achievements,
          statistics: history.statistics,
          pagination: {
            page: parseInt(page),
            limit: parseInt(limit),
            total: history.cvVersions.length,
            totalPages: Math.ceil(history.cvVersions.length / limit),
          },
        },
      });
    } catch (error) {
      logger.error("Failed to get feedback history:", error);
      res.status(500).json({
        success: false,
        message: "Failed to retrieve feedback history",
        error:
          process.env.NODE_ENV === "development"
            ? error.message
            : "Internal server error",
      });
    }
  }

  // Get user's progress dashboard
  async getProgressDashboard(req, res) {
    try {
      const { userId } = req.params;

      const history = await FeedbackHistory.findOne({ userId });

      if (!history) {
        return res.status(404).json({
          success: false,
          message: "No progress data found for this user",
        });
      }

      // Get recent CVs with feedback
      const recentCVs = await CV.find({ userId })
        .sort({ createdAt: -1 })
        .limit(5)
        .populate({
          path: "userId",
          select: "fileName fileType status createdAt",
        });

      const recentCVsWithFeedback = await Promise.all(
        recentCVs.map(async (cv) => {
          const latestFeedback = await AIFeedback.findOne({
            cvId: cv._id,
          }).sort({ createdAt: -1 });

          return {
            cvId: cv._id,
            fileName: cv.fileName,
            status: cv.status,
            createdAt: cv.createdAt,
            overallScore: latestFeedback
              ? latestFeedback.feedback.overallScore
              : null,
            version: latestFeedback ? latestFeedback.version : null,
          };
        })
      );

      // Calculate improvement trends
      const improvementTrends = this.calculateImprovementTrends(history);

      // Generate insights and recommendations
      const insights = this.generateInsights(history);

      res.json({
        success: true,
        data: {
          userId,
          summary: {
            totalCVsAnalyzed: history.statistics.totalCVsAnalyzed,
            currentScore: history.progressMetrics.currentScore,
            bestScore: history.progressMetrics.bestScore,
            totalImprovement:
              history.progressMetrics.currentScore -
              history.progressMetrics.initialScore,
            currentStreak: history.progressMetrics.streaks.currentStreak,
            longestStreak: history.progressMetrics.streaks.longestStreak,
          },
          recentCVs: recentCVsWithFeedback,
          improvementTrends,
          improvementAreas: history.progressMetrics.improvementAreas,
          achievements: history.achievements.slice(-5), // Latest 5 achievements
          insights,
          nextGoals: this.generateNextGoals(history),
        },
      });
    } catch (error) {
      logger.error("Failed to get progress dashboard:", error);
      res.status(500).json({
        success: false,
        message: "Failed to retrieve progress dashboard",
        error:
          process.env.NODE_ENV === "development"
            ? error.message
            : "Internal server error",
      });
    }
  }

  // Apply suggestion and update history
  async applySuggestion(req, res) {
    try {
      const { userId } = req.params;
      const { suggestionId, suggestionText, cvId } = req.body;

      if (!suggestionId || !suggestionText) {
        return res.status(400).json({
          success: false,
          message: "Suggestion ID and text are required",
        });
      }

      let history = await FeedbackHistory.findOne({ userId });

      if (!history) {
        return res.status(404).json({
          success: false,
          message: "No feedback history found for this user",
        });
      }

      // Apply suggestion to history
      await history.applySuggestion({
        suggestionId,
        suggestionText,
        cvId,
        appliedAt: new Date(),
      });

      // Check for new achievements
      const newAchievements = this.checkForAchievements(history);

      if (newAchievements.length > 0) {
        history.achievements.push(...newAchievements);
        await history.save();
      }

      logger.info("Suggestion applied to history", {
        userId,
        suggestionId,
        currentStreak: history.progressMetrics.streaks.currentStreak,
      });

      res.json({
        success: true,
        message: "Suggestion applied successfully",
        data: {
          suggestionId,
          currentStreak: history.progressMetrics.streaks.currentStreak,
          totalImprovements: history.progressMetrics.totalImprovements,
          newAchievements,
        },
      });
    } catch (error) {
      logger.error("Failed to apply suggestion to history:", error);
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

  // Get improvement analytics
  async getImprovementAnalytics(req, res) {
    try {
      const { userId } = req.params;
      const { period = "30d" } = req.query;

      const history = await FeedbackHistory.findOne({ userId });

      if (!history) {
        return res.status(404).json({
          success: false,
          message: "No analytics data found for this user",
        });
      }

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

      // Filter data by period
      const periodVersions = history.cvVersions.filter(
        (version) => new Date(version.timestamp) >= startDate
      );

      // Calculate analytics
      const analytics = {
        period,
        totalVersionsInPeriod: periodVersions.length,
        averageScore:
          periodVersions.length > 0
            ? periodVersions.reduce((sum, v) => sum + v.overallScore, 0) /
              periodVersions.length
            : 0,
        scoreImprovement:
          periodVersions.length > 1
            ? periodVersions[periodVersions.length - 1].overallScore -
              periodVersions[0].overallScore
            : 0,
        suggestionsApplied: periodVersions.reduce(
          (sum, v) => sum + v.appliedSuggestions.length,
          0
        ),
        mostImprovedSection: this.getMostImprovedSection(periodVersions),
        improvementVelocity: this.calculateImprovementVelocity(periodVersions),
        consistencyScore: this.calculateConsistencyScore(history, startDate),
      };

      res.json({
        success: true,
        data: analytics,
      });
    } catch (error) {
      logger.error("Failed to get improvement analytics:", error);
      res.status(500).json({
        success: false,
        message: "Failed to retrieve improvement analytics",
        error:
          process.env.NODE_ENV === "development"
            ? error.message
            : "Internal server error",
      });
    }
  }

  // Compare CV versions
  async compareVersions(req, res) {
    try {
      const { userId } = req.params;
      const { cvId, fromVersion, toVersion } = req.query;

      if (!cvId || !fromVersion || !toVersion) {
        return res.status(400).json({
          success: false,
          message: "CV ID, from version, and to version are required",
        });
      }

      // Get feedback for both versions
      const [fromFeedback, toFeedback] = await Promise.all([
        AIFeedback.findOne({ cvId, version: parseInt(fromVersion) }),
        AIFeedback.findOne({ cvId, version: parseInt(toVersion) }),
      ]);

      if (!fromFeedback || !toFeedback) {
        return res.status(404).json({
          success: false,
          message: "One or both feedback versions not found",
        });
      }

      // Calculate comparison data
      const comparison = {
        fromVersion: {
          version: fromFeedback.version,
          overallScore: fromFeedback.feedback.overallScore,
          sectionScores: this.extractSectionScores(
            fromFeedback.feedback.sectionAnalysis
          ),
          createdAt: fromFeedback.createdAt,
        },
        toVersion: {
          version: toFeedback.version,
          overallScore: toFeedback.feedback.overallScore,
          sectionScores: this.extractSectionScores(
            toFeedback.feedback.sectionAnalysis
          ),
          createdAt: toFeedback.createdAt,
        },
        improvements: {
          overallScoreChange:
            toFeedback.feedback.overallScore -
            fromFeedback.feedback.overallScore,
          sectionChanges: this.calculateSectionChanges(
            fromFeedback.feedback.sectionAnalysis,
            toFeedback.feedback.sectionAnalysis
          ),
          appliedSuggestions: this.getAppliedSuggestionsBetweenVersions(
            fromFeedback.feedback.improvementSuggestions,
            toFeedback.feedback.improvementSuggestions
          ),
        },
      };

      res.json({
        success: true,
        data: comparison,
      });
    } catch (error) {
      logger.error("Failed to compare versions:", error);
      res.status(500).json({
        success: false,
        message: "Failed to compare CV versions",
        error:
          process.env.NODE_ENV === "development"
            ? error.message
            : "Internal server error",
      });
    }
  }

  // Helper methods
  calculateProgressMetrics(history) {
    const versions = history.cvVersions.sort(
      (a, b) => new Date(a.timestamp) - new Date(b.timestamp)
    );

    return {
      scoreProgression: versions.map((v) => ({
        version: v.version,
        score: v.overallScore,
        date: v.timestamp,
      })),
      improvementRate:
        versions.length > 1
          ? (versions[versions.length - 1].overallScore -
              versions[0].overallScore) /
            versions.length
          : 0,
      averageImprovement: history.statistics.averageScoreImprovement,
    };
  }

  calculateImprovementTrends(history) {
    const versions = history.cvVersions.sort(
      (a, b) => new Date(a.timestamp) - new Date(b.timestamp)
    );
    const trends = [];

    for (let i = 1; i < versions.length; i++) {
      trends.push({
        period: `v${versions[i - 1].version} → v${versions[i].version}`,
        scoreChange: versions[i].overallScore - versions[i - 1].overallScore,
        date: versions[i].timestamp,
      });
    }

    return trends;
  }

  generateInsights(history) {
    const insights = [];
    const currentScore = history.progressMetrics.currentScore;
    const initialScore = history.progressMetrics.initialScore;
    const improvement = currentScore - initialScore;

    if (improvement > 20) {
      insights.push({
        type: "achievement",
        message: `Excellent progress! You've improved your CV score by ${improvement.toFixed(
          1
        )} points.`,
        priority: "high",
      });
    }

    if (history.progressMetrics.streaks.currentStreak >= 5) {
      insights.push({
        type: "streak",
        message: `You're on a ${history.progressMetrics.streaks.currentStreak}-day improvement streak! Keep it up!`,
        priority: "medium",
      });
    }

    if (
      history.statistics.totalSuggestionsApplied <
      history.statistics.totalSuggestionsReceived * 0.3
    ) {
      insights.push({
        type: "suggestion",
        message:
          "Consider applying more AI suggestions to accelerate your improvement.",
        priority: "medium",
      });
    }

    return insights;
  }

  generateNextGoals(history) {
    const goals = [];
    const currentScore = history.progressMetrics.currentScore;

    if (currentScore < 70) {
      goals.push({
        title: "Reach 70+ Score",
        description: "Focus on improving your weakest sections",
        target: 70,
        current: currentScore,
      });
    }

    if (history.progressMetrics.streaks.currentStreak < 7) {
      goals.push({
        title: "Build a 7-Day Streak",
        description: "Make daily improvements to your CV",
        target: 7,
        current: history.progressMetrics.streaks.currentStreak,
      });
    }

    return goals;
  }

  checkForAchievements(history) {
    const achievements = [];
    const stats = history.statistics;
    const metrics = history.progressMetrics;

    // Check for score milestones
    if (
      metrics.currentScore >= 90 &&
      !this.hasAchievement(history, "score_90")
    ) {
      achievements.push({
        type: "score_milestone",
        title: "CV Master",
        description: "Achieved a CV score of 90+",
        criteria: { scoreThreshold: 90 },
      });
    }

    // Check for improvement streak
    if (
      metrics.streaks.currentStreak >= 10 &&
      !this.hasAchievement(history, "streak_10")
    ) {
      achievements.push({
        type: "improvement_streak",
        title: "Consistent Improver",
        description: "Maintained a 10-day improvement streak",
        criteria: { streakLength: 10 },
      });
    }

    return achievements;
  }

  hasAchievement(history, achievementCode) {
    return history.achievements.some(
      (achievement) =>
        achievement.criteria && achievement.criteria.code === achievementCode
    );
  }

  extractSectionScores(sectionAnalysis) {
    const sections = {};
    for (const [section, data] of Object.entries(sectionAnalysis)) {
      sections[section] = data.score || 0;
    }
    return sections;
  }

  calculateSectionChanges(fromAnalysis, toAnalysis) {
    const changes = {};
    for (const section of Object.keys(fromAnalysis)) {
      const fromScore = fromAnalysis[section]?.score || 0;
      const toScore = toAnalysis[section]?.score || 0;
      changes[section] = toScore - fromScore;
    }
    return changes;
  }

  getAppliedSuggestionsBetweenVersions(fromSuggestions, toSuggestions) {
    const appliedSuggestions = [];

    for (const suggestion of fromSuggestions) {
      const updatedSuggestion = toSuggestions.find(
        (s) => s.id === suggestion.id
      );
      if (
        updatedSuggestion &&
        updatedSuggestion.applied &&
        !suggestion.applied
      ) {
        appliedSuggestions.push({
          id: suggestion.id,
          suggestion: suggestion.suggestion,
          appliedAt: updatedSuggestion.appliedAt,
        });
      }
    }

    return appliedSuggestions;
  }

  getMostImprovedSection(versions) {
    if (versions.length < 2) return null;

    const sectionImprovements = {};
    const sections = [
      "careerObjective",
      "skills",
      "experience",
      "education",
      "projects",
      "formatting",
    ];

    // Calculate average improvement per section
    sections.forEach((section) => {
      const improvements = [];
      for (let i = 1; i < versions.length; i++) {
        const prevScore = versions[i - 1].sectionScores?.[section] || 0;
        const currScore = versions[i].sectionScores?.[section] || 0;
        improvements.push(currScore - prevScore);
      }
      sectionImprovements[section] =
        improvements.length > 0
          ? improvements.reduce((a, b) => a + b, 0) / improvements.length
          : 0;
    });

    // Find section with highest improvement
    const mostImproved = Object.entries(sectionImprovements).sort(
      ([, a], [, b]) => b - a
    )[0];

    return mostImproved
      ? {
          section: mostImproved[0],
          improvement: mostImproved[1],
        }
      : null;
  }

  calculateImprovementVelocity(versions) {
    if (versions.length < 2) return 0;

    const timePeriod =
      new Date(versions[versions.length - 1].timestamp) -
      new Date(versions[0].timestamp);
    const scoreImprovement =
      versions[versions.length - 1].overallScore - versions[0].overallScore;

    // Return points per day
    return timePeriod > 0
      ? scoreImprovement / (timePeriod / (1000 * 60 * 60 * 24))
      : 0;
  }

  calculateConsistencyScore(history, startDate) {
    const recentVersions = history.cvVersions
      .filter((v) => new Date(v.timestamp) >= startDate)
      .sort((a, b) => new Date(a.timestamp) - new Date(b.timestamp));

    if (recentVersions.length < 2) return 0;

    // Calculate consistency based on regular improvements
    const daysBetweenVersions = [];
    for (let i = 1; i < recentVersions.length; i++) {
      const days =
        (new Date(recentVersions[i].timestamp) -
          new Date(recentVersions[i - 1].timestamp)) /
        (1000 * 60 * 60 * 24);
      daysBetweenVersions.push(days);
    }

    // Lower variance = higher consistency
    const avgDays =
      daysBetweenVersions.reduce((a, b) => a + b, 0) /
      daysBetweenVersions.length;
    const variance =
      daysBetweenVersions.reduce((a, b) => a + Math.pow(b - avgDays, 2), 0) /
      daysBetweenVersions.length;

    return Math.max(0, 100 - variance); // Convert to 0-100 scale
  }
}

module.exports = new FeedbackController();
