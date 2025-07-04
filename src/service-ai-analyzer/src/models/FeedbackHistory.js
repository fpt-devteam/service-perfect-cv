const mongoose = require("mongoose");

const feedbackHistorySchema = new mongoose.Schema(
  {
    userId: {
      type: String,
      required: true,
      unique: true,
      index: true,
    },
    cvVersions: [
      {
        cvId: {
          type: mongoose.Schema.Types.ObjectId,
          ref: "CV",
          required: true,
        },
        feedbackId: {
          type: mongoose.Schema.Types.ObjectId,
          ref: "AIFeedback",
          required: true,
        },
        version: {
          type: Number,
          required: true,
        },
        overallScore: {
          type: Number,
          min: 0,
          max: 100,
        },
        changes: [String], // What was changed from previous version
        appliedSuggestions: [
          {
            suggestionId: String,
            suggestionText: String,
            appliedAt: Date,
          },
        ],
        timestamp: {
          type: Date,
          default: Date.now,
        },
      },
    ],
    progressMetrics: {
      initialScore: {
        type: Number,
        default: 0,
      },
      currentScore: {
        type: Number,
        default: 0,
      },
      bestScore: {
        type: Number,
        default: 0,
      },
      totalImprovements: {
        type: Number,
        default: 0,
      },
      improvementAreas: [
        {
          section: String,
          initialScore: Number,
          currentScore: Number,
          improvement: Number,
          lastUpdated: Date,
        },
      ],
      streaks: {
        currentStreak: {
          type: Number,
          default: 0,
        },
        longestStreak: {
          type: Number,
          default: 0,
        },
        lastActivityDate: Date,
      },
    },
    preferences: {
      targetIndustry: String,
      targetRole: String,
      experienceLevel: {
        type: String,
        enum: ["entry", "mid", "senior", "executive"],
      },
      notificationPreferences: {
        improvementReminders: {
          type: Boolean,
          default: true,
        },
        weeklyProgress: {
          type: Boolean,
          default: true,
        },
      },
    },
    achievements: [
      {
        type: {
          type: String,
          enum: [
            "first_cv",
            "score_milestone",
            "improvement_streak",
            "section_master",
          ],
        },
        title: String,
        description: String,
        unlockedAt: {
          type: Date,
          default: Date.now,
        },
        criteria: mongoose.Schema.Types.Mixed,
      },
    ],
    statistics: {
      totalCVsAnalyzed: {
        type: Number,
        default: 0,
      },
      totalSuggestionsReceived: {
        type: Number,
        default: 0,
      },
      totalSuggestionsApplied: {
        type: Number,
        default: 0,
      },
      averageScoreImprovement: {
        type: Number,
        default: 0,
      },
      mostImprovedSection: String,
      timeSpentImproving: {
        type: Number, // in minutes
        default: 0,
      },
    },
  },
  {
    timestamps: true,
  }
);

// Methods to update progress
feedbackHistorySchema.methods.addCVVersion = function (cvData) {
  this.cvVersions.push(cvData);
  this.statistics.totalCVsAnalyzed += 1;

  // Update progress metrics
  if (this.progressMetrics.initialScore === 0) {
    this.progressMetrics.initialScore = cvData.overallScore;
  }
  this.progressMetrics.currentScore = cvData.overallScore;
  if (cvData.overallScore > this.progressMetrics.bestScore) {
    this.progressMetrics.bestScore = cvData.overallScore;
  }

  return this.save();
};

feedbackHistorySchema.methods.applySuggestion = function (suggestionData) {
  this.statistics.totalSuggestionsApplied += 1;
  this.progressMetrics.totalImprovements += 1;

  // Update streak
  const today = new Date();
  const lastActivity = this.progressMetrics.streaks.lastActivityDate;

  if (lastActivity && this.isSameDay(today, lastActivity)) {
    // Same day, no streak change
  } else if (lastActivity && this.isConsecutiveDay(today, lastActivity)) {
    this.progressMetrics.streaks.currentStreak += 1;
    if (
      this.progressMetrics.streaks.currentStreak >
      this.progressMetrics.streaks.longestStreak
    ) {
      this.progressMetrics.streaks.longestStreak =
        this.progressMetrics.streaks.currentStreak;
    }
  } else {
    this.progressMetrics.streaks.currentStreak = 1;
  }

  this.progressMetrics.streaks.lastActivityDate = today;
  return this.save();
};

feedbackHistorySchema.methods.isSameDay = function (date1, date2) {
  return date1.toDateString() === date2.toDateString();
};

feedbackHistorySchema.methods.isConsecutiveDay = function (date1, date2) {
  const diffTime = Math.abs(date1 - date2);
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
  return diffDays === 1;
};

// Indexes for performance
feedbackHistorySchema.index({ userId: 1 });
feedbackHistorySchema.index({ "progressMetrics.currentScore": -1 });
feedbackHistorySchema.index({ "cvVersions.timestamp": -1 });

module.exports = mongoose.model("FeedbackHistory", feedbackHistorySchema);
