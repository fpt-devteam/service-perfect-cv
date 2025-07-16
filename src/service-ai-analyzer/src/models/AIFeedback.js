const mongoose = require("mongoose");

const aiFeedbackSchema = new mongoose.Schema(
  {
    // Reference to CV analysis record
    cvAnalysisId: {
      type: mongoose.Schema.Types.ObjectId,
      ref: "CVAnalysis",
      required: true,
      index: true,
    },

    // Original CV ID from main system
    cvId: {
      type: String, // UUID from main system
      required: true,
      index: true,
    },

    userId: {
      type: String, // UUID from main system
      required: true,
      index: true,
    },

    // Complete AI analysis results
    feedback: {
      // Overall score (0-100)
      overallScore: {
        type: Number,
        min: 0,
        max: 100,
        required: true,
      },

      // Section-by-section analysis
      sectionAnalysis: {
        // Career Objective/Summary analysis
        summary: {
          score: { type: Number, min: 0, max: 100 },
          feedback: String,
          suggestions: [String],
          strengths: [String],
          weaknesses: [String],
        },

        // Skills analysis
        skills: {
          score: { type: Number, min: 0, max: 100 },
          feedback: String,
          suggestions: [String],
          strengths: [String],
          weaknesses: [String],
          missingSkills: [String],
          relevantSkills: [String],
        },

        // Experience analysis
        experience: {
          score: { type: Number, min: 0, max: 100 },
          feedback: String,
          suggestions: [String],
          strengths: [String],
          weaknesses: [String],
          careerProgression: String,
          quantificationLevel: String,
        },

        // Education analysis
        education: {
          score: { type: Number, min: 0, max: 100 },
          feedback: String,
          suggestions: [String],
          strengths: [String],
          weaknesses: [String],
          relevance: String,
        },

        // Projects analysis
        projects: {
          score: { type: Number, min: 0, max: 100 },
          feedback: String,
          suggestions: [String],
          strengths: [String],
          weaknesses: [String],
          technicalDepth: String,
          businessImpact: String,
        },

        // Certifications analysis
        certifications: {
          score: { type: Number, min: 0, max: 100 },
          feedback: String,
          suggestions: [String],
          strengths: [String],
          weaknesses: [String],
          industryRelevance: String,
        },

        // Overall formatting and presentation
        formatting: {
          score: { type: Number, min: 0, max: 100 },
          feedback: String,
          suggestions: [String],
          strengths: [String],
          weaknesses: [String],
        },
      },

      // Improvement suggestions
      improvementSuggestions: [
        {
          id: { type: String, required: true },
          section: {
            type: String,
            enum: ["summary", "skills", "experience", "education", "projects", "certifications", "formatting", "general"],
            required: true,
          },
          priority: {
            type: String,
            enum: ["low", "medium", "high", "critical"],
            required: true,
          },
          suggestion: { type: String, required: true },
          explanation: String,
          autoApplicable: {
            type: Boolean,
            default: false,
          },
          category: {
            type: String,
            enum: ["content", "structure", "technical", "formatting", "keywords"],
            required: true,
          },
          estimatedImpact: {
            type: String,
            enum: ["low", "medium", "high"],
            default: "medium",
          },
          appliedAt: Date,
          appliedBy: String,
        },
      ],

      // Industry-specific advice
      industrySpecificAdvice: {
        targetIndustry: String,
        relevanceScore: { type: Number, min: 0, max: 100 },
        industryTrends: [String],
        recommendedSkills: [String],
        industrySpecificSuggestions: [String],
        competitorAnalysis: String,
        marketDemand: String,
      },

      // ATS (Applicant Tracking System) compatibility
      atsCompatibility: {
        score: { type: Number, min: 0, max: 100 },
        issues: [String],
        recommendations: [String],
        keywordDensity: Number,
        formatCompliance: String,
        scanability: String,
      },
    },

    // Processing metadata
    processingTime: {
      type: Number, // Time in milliseconds
      required: true,
    },

    // Version tracking for improvements
    version: {
      type: Number,
      default: 1,
    },

    // Raw AI response for debugging
    mistralResponse: {
      type: mongoose.Schema.Types.Mixed,
      required: true,
    },

    // AI model info
    modelInfo: {
      model: String,
      temperature: Number,
      tokensUsed: Number,
      requestId: String,
    },
  },
  {
    timestamps: true,
  }
);

// Indexes for performance
aiFeedbackSchema.index({ cvAnalysisId: 1, createdAt: -1 });
aiFeedbackSchema.index({ cvId: 1, createdAt: -1 });
aiFeedbackSchema.index({ userId: 1, createdAt: -1 });
aiFeedbackSchema.index({ version: 1 });

// Compound indexes
aiFeedbackSchema.index({ cvId: 1, version: 1 });
aiFeedbackSchema.index({ userId: 1, "feedback.overallScore": -1 });

module.exports = mongoose.model("AIFeedback", aiFeedbackSchema);

