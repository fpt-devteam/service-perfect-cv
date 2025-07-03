const mongoose = require("mongoose");

// CV Analysis Record - tracks analysis requests for CVs managed by other service
const cvAnalysisSchema = new mongoose.Schema(
  {
    // Reference to CV in main system
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

    // CV data received from main system (structured JSON)
    cvData: {
      // Basic CV info
      title: String,

      // Contact information
      contact: {
        phoneNumber: String,
        email: String,
        linkedInUrl: String,
        githubUrl: String,
        personalWebsiteUrl: String,
        country: String,
        city: String,
      },

      // Summary/Career Objective
      summary: {
        context: String,
      },

      // Education
      education: [
        {
          degree: String,
          degreeId: String,
          organization: String,
          organizationId: String,
          fieldOfStudy: String,
          startDate: Date,
          endDate: Date,
          gpa: Number,
          description: String,
        },
      ],

      // Experience
      experience: [
        {
          jobTitle: String,
          jobTitleId: String,
          employmentTypeId: String,
          company: String,
          companyId: String,
          location: String,
          startDate: Date,
          endDate: Date,
          description: String,
        },
      ],

      // Projects
      projects: [
        {
          title: String,
          description: String,
          link: String,
          startDate: Date,
          endDate: Date,
        },
      ],

      // Skills (stored as JSON)
      skills: [
        {
          category: String,
          items: [String], // Parsed from itemsJson
        },
      ],

      // Certifications
      certifications: [
        {
          name: String,
          organization: String,
          organizationId: String,
          issuedDate: Date,
          relevance: String,
        },
      ],

      // Template information
      template: {
        id: String,
        name: String,
        descriptor: mongoose.Schema.Types.Mixed, // jsonb
        cssUrl: String,
        reactBundle: String,
        previewUrl: String,
      },
    },

    // Analysis processing status
    status: {
      type: String,
      enum: ["pending", "processing", "completed", "failed"],
      default: "pending",
    },

    // Queue job tracking
    processingJobId: {
      type: String,
      default: null,
    },

    // User preferences for analysis
    userPreferences: {
      targetIndustry: String,
      targetRole: String,
      experienceLevel: String,
      focusAreas: [String], // e.g., ["technical-skills", "leadership", "achievements"]
      urgent: {
        type: Boolean,
        default: false,
      },
    },

    // Analysis request metadata
    requestMetadata: {
      source: {
        type: String,
        default: "main-cv-service",
      },
      version: String, // CV version from main system
      submittedAt: Date,
    },
  },
  {
    timestamps: true,
  }
);

// Indexes for performance
cvAnalysisSchema.index({ cvId: 1, createdAt: -1 });
cvAnalysisSchema.index({ userId: 1, createdAt: -1 });
cvAnalysisSchema.index({ status: 1 });
cvAnalysisSchema.index({ processingJobId: 1 });

// Compound index for finding latest analysis for a CV
cvAnalysisSchema.index({ cvId: 1, userId: 1, createdAt: -1 });

module.exports = mongoose.model("CVAnalysis", cvAnalysisSchema);
