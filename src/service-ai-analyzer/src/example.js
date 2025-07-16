/**
 * CV Analysis Microservice API Examples
 *
 * This file demonstrates how to interact with the CV Analysis API
 * using structured CV data from the main CV management system.
 */

// Example 1: Submit CV for Analysis
const exampleCVAnalysisRequest = {
  // Required: IDs from main CV system
  cvId: "123e4567-e89b-12d3-a456-426614174000", // UUID from main CV system
  userId: "987fcdeb-51a2-43d1-9c40-162516281234", // UUID from main CV system

  // Required: Structured CV data
  cvData: {
    title: "Senior Software Engineer CV",

    // Contact information
    contact: {
      phoneNumber: "+1-555-123-4567",
      email: "john.doe@email.com",
      linkedInUrl: "https://linkedin.com/in/johndoe",
      githubUrl: "https://github.com/johndoe",
      personalWebsiteUrl: "https://johndoe.dev",
      country: "United States",
      city: "San Francisco",
    },

    // Professional summary
    summary: {
      context:
        "Experienced software engineer with 5+ years developing scalable web applications using React, Node.js, and cloud technologies. Passionate about clean code, testing, and agile methodologies.",
    },

    // Experience history
    experience: [
      {
        jobTitle: "Senior Software Engineer",
        jobTitleId: "456e7890-e12b-34d5-a678-901234567890",
        employmentTypeId: "full-time-uuid",
        company: "Tech Corp",
        companyId: "789e0123-e45f-67g8-h901-234567890123",
        location: "San Francisco, CA",
        startDate: "2020-01-15",
        endDate: null, // Current position
        description:
          "Led development of microservices architecture serving 1M+ users. Improved system performance by 40% through optimization. Mentored 3 junior developers.",
      },
      {
        jobTitle: "Software Engineer",
        jobTitleId: "456e7890-e12b-34d5-a678-901234567890",
        employmentTypeId: "full-time-uuid",
        company: "StartupXYZ",
        companyId: "abc1234-def5-678g-h901-234567890abc",
        location: "Remote",
        startDate: "2018-06-01",
        endDate: "2019-12-31",
        description:
          "Built responsive web applications using React and Node.js. Implemented CI/CD pipelines reducing deployment time by 60%.",
      },
    ],

    // Education background
    education: [
      {
        degree: "Bachelor of Science",
        degreeId: "bs-degree-uuid",
        organization: "Stanford University",
        organizationId: "stanford-uuid",
        fieldOfStudy: "Computer Science",
        startDate: "2014-09-01",
        endDate: "2018-05-15",
        gpa: 3.8,
        description:
          "Specialized in algorithms and data structures. Dean's List for 3 semesters.",
      },
    ],

    // Technical skills
    skills: [
      {
        category: "Programming Languages",
        items: ["JavaScript", "TypeScript", "Python", "Java", "Go"],
      },
      {
        category: "Frontend",
        items: ["React", "Vue.js", "HTML5", "CSS3", "Sass"],
      },
      {
        category: "Backend",
        items: ["Node.js", "Express", "Django", "Spring Boot"],
      },
      {
        category: "Databases",
        items: ["MongoDB", "PostgreSQL", "Redis", "Elasticsearch"],
      },
      {
        category: "Cloud & DevOps",
        items: ["AWS", "Docker", "Kubernetes", "CI/CD", "Terraform"],
      },
    ],

    // Personal projects
    projects: [
      {
        title: "E-commerce Platform",
        description:
          "Full-stack e-commerce platform with microservices architecture, payment integration, and real-time analytics dashboard.",
        link: "https://github.com/johndoe/ecommerce-platform",
        startDate: "2021-03-01",
        endDate: "2021-08-15",
      },
      {
        title: "AI Task Manager",
        description:
          "Task management app with AI-powered priority suggestions and natural language processing for task creation.",
        link: "https://taskmanager.johndoe.dev",
        startDate: "2022-01-01",
        endDate: null, // Ongoing
      },
    ],

    // Professional certifications
    certifications: [
      {
        name: "AWS Solutions Architect Associate",
        organization: "Amazon Web Services",
        organizationId: "aws-uuid",
        issuedDate: "2021-09-15",
        relevance:
          "Validates expertise in designing distributed systems on AWS",
      },
      {
        name: "Certified Kubernetes Application Developer",
        organization: "CNCF",
        organizationId: "cncf-uuid",
        issuedDate: "2022-03-20",
        relevance:
          "Demonstrates proficiency in Kubernetes application development",
      },
    ],

    // Template information (from CV builder)
    template: {
      id: "modern-tech-template-uuid",
      name: "Modern Tech Template",
      descriptor: {
        style: "modern",
        layout: "two-column",
        colorScheme: "blue",
      },
      cssUrl: "https://templates.cvservice.com/modern-tech.css",
      previewUrl: "https://templates.cvservice.com/previews/modern-tech.png",
    },
  },

  // Optional: User preferences for analysis
  userPreferences: {
    targetIndustry: "Software Technology",
    targetRole: "Senior Software Engineer",
    experienceLevel: "senior",
    focusAreas: ["technical-skills", "leadership", "achievements"],
    urgent: false,
  },

  // Optional: Request metadata
  requestMetadata: {
    source: "cv-builder-app",
    version: "1.2.3",
    submittedAt: "2023-12-01T10:30:00Z",
  },
};

// Example 2: API Request using axios
const axios = require("axios");

async function submitCVForAnalysis() {
  try {
    const response = await axios.post(
      "http://localhost:3001/api/cv/analyze",
      exampleCVAnalysisRequest,
      {
        headers: {
          "Content-Type": "application/json",
          // Add authentication headers in production
          // 'Authorization': 'Bearer service-token'
        },
      }
    );

    console.log("Analysis submitted successfully:", response.data);

    // Response will include:
    // {
    //   success: true,
    //   analysisId: "mongodb-objectid",
    //   jobId: "bull-queue-job-id",
    //   message: "CV analysis started",
    //   estimatedCompletionTime: "2023-12-01T10:35:00Z",
    //   status: "pending"
    // }

    return response.data;
  } catch (error) {
    console.error(
      "Error submitting CV analysis:",
      error.response?.data || error.message
    );
    throw error;
  }
}

// Example 3: Check Analysis Status
async function checkAnalysisStatus(analysisId) {
  try {
    const response = await axios.get(
      `http://localhost:3001/api/cv/analysis/${analysisId}/status`
    );

    console.log("Analysis status:", response.data);

    // Response includes:
    // {
    //   analysisId: "mongodb-objectid",
    //   status: "processing|completed|failed",
    //   progress: 75,
    //   estimatedTimeRemaining: "2 minutes",
    //   startedAt: "2023-12-01T10:30:00Z",
    //   completedAt: null | "2023-12-01T10:35:00Z"
    // }

    return response.data;
  } catch (error) {
    console.error(
      "Error checking status:",
      error.response?.data || error.message
    );
    throw error;
  }
}

// Example 4: Get Analysis Results/Feedback
async function getAnalysisFeedback(analysisId, version = null) {
  try {
    const url = version
      ? `http://localhost:3001/api/cv/${analysisId}/feedback?version=${version}`
      : `http://localhost:3001/api/cv/${analysisId}/feedback`;

    const response = await axios.get(url);

    console.log("Analysis feedback:", response.data);

    // Response includes comprehensive AI feedback:
    // {
    //   analysisId: "mongodb-objectid",
    //   feedback: {
    //     overallScore: 85,
    //     sectionAnalysis: {
    //       summary: { score: 80, feedback: "...", suggestions: [...] },
    //       skills: { score: 90, feedback: "...", suggestions: [...] },
    //       experience: { score: 85, feedback: "...", suggestions: [...] },
    //       // ... other sections
    //     },
    //     improvementSuggestions: [
    //       {
    //         id: "suggestion-uuid",
    //         section: "summary",
    //         priority: "high",
    //         suggestion: "Add quantifiable achievements...",
    //         explanation: "Metrics make your impact more compelling...",
    //         autoApplicable: false,
    //         category: "content",
    //         estimatedImpact: "high"
    //       }
    //     ],
    //     industrySpecificAdvice: { ... },
    //     atsCompatibility: { ... }
    //   },
    //   version: 1,
    //   createdAt: "2023-12-01T10:35:00Z"
    // }

    return response.data;
  } catch (error) {
    console.error(
      "Error getting feedback:",
      error.response?.data || error.message
    );
    throw error;
  }
}

// Example 5: Reanalyze CV (when CV data is updated in main system)
async function reanalyzeCVWithUpdatedData(
  analysisId,
  updatedCvData,
  reason = "cv_updated"
) {
  try {
    const response = await axios.post(
      `http://localhost:3001/api/cv/analysis/${analysisId}/reanalyze`,
      {
        updatedCvData: updatedCvData, // New structured CV data
        reason: reason,
        userPreferences: {
          targetIndustry: "Software Technology",
          targetRole: "Senior Software Engineer",
          experienceLevel: "senior",
        },
      }
    );

    console.log("Reanalysis started:", response.data);
    return response.data;
  } catch (error) {
    console.error(
      "Error starting reanalysis:",
      error.response?.data || error.message
    );
    throw error;
  }
}

// Example 6: Get Analysis History for a CV
async function getCVAnalysisHistory(cvId, userId) {
  try {
    const response = await axios.get(
      `http://localhost:3001/api/cv/${cvId}/analysis-history?userId=${userId}&page=1&limit=10`
    );

    console.log("CV analysis history:", response.data);

    // Response includes:
    // {
    //   cvId: "uuid",
    //   analyses: [
    //     {
    //       analysisId: "objectid",
    //       status: "completed",
    //       overallScore: 85,
    //       version: 2,
    //       createdAt: "2023-12-01T10:35:00Z",
    //       feedback: { ... }
    //     }
    //   ],
    //   pagination: { page: 1, limit: 10, total: 3, pages: 1 }
    // }

    return response.data;
  } catch (error) {
    console.error(
      "Error getting analysis history:",
      error.response?.data || error.message
    );
    throw error;
  }
}

// Export examples for testing
module.exports = {
  exampleCVAnalysisRequest,
  submitCVForAnalysis,
  checkAnalysisStatus,
  getAnalysisFeedback,
  reanalyzeCVWithUpdatedData,
  getCVAnalysisHistory,
};

// Example usage:
if (require.main === module) {
  (async () => {
    try {
      console.log("📝 Submitting CV for analysis...");
      const result = await submitCVForAnalysis();
      const analysisId = result.analysisId;

      console.log("⏳ Checking status...");
      await new Promise((resolve) => setTimeout(resolve, 2000)); // Wait 2 seconds
      await checkAnalysisStatus(analysisId);

      // In a real scenario, you'd poll until completion
      console.log("✅ Getting feedback (when ready)...");
      // await getAnalysisFeedback(analysisId);
    } catch (error) {
      console.error("❌ Example failed:", error.message);
    }
  })();
}
