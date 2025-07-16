const mongoose = require("mongoose");

const qaInteractionSchema = new mongoose.Schema(
  {
    userId: {
      type: String,
      required: true,
      index: true,
    },
    cvId: {
      type: mongoose.Schema.Types.ObjectId,
      ref: "CV",
      default: null, // Optional, for context-specific questions
    },
    sessionId: {
      type: String,
      required: true,
      index: true,
    },
    conversation: [
      {
        role: {
          type: String,
          enum: ["user", "assistant"],
          required: true,
        },
        message: {
          type: String,
          required: true,
        },
        timestamp: {
          type: Date,
          default: Date.now,
        },
        metadata: {
          type: mongoose.Schema.Types.Mixed,
          default: {},
        },
      },
    ],
    context: {
      cvSummary: String, // Brief CV context for AI
      previousTopics: [String], // Track conversation themes
      userPreferences: {
        industry: String,
        experienceLevel: String,
        targetRole: String,
      },
    },
    lastActivity: {
      type: Date,
      default: Date.now,
    },
    status: {
      type: String,
      enum: ["active", "closed", "archived"],
      default: "active",
    },
    totalMessages: {
      type: Number,
      default: 0,
    },
    avgResponseTime: {
      type: Number, // in milliseconds
      default: 0,
    },
  },
  {
    timestamps: true,
  }
);

// Update lastActivity on conversation updates
qaInteractionSchema.pre("save", function (next) {
  this.lastActivity = new Date();
  this.totalMessages = this.conversation.length;
  next();
});

// Indexes for performance
qaInteractionSchema.index({ userId: 1, lastActivity: -1 });
qaInteractionSchema.index({ sessionId: 1 });
qaInteractionSchema.index({ cvId: 1 });
qaInteractionSchema.index({ status: 1 });

module.exports = mongoose.model("QAInteraction", qaInteractionSchema);
