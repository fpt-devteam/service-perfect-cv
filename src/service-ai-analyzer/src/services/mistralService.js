const axios = require("axios");
const logger = require("../config/logger");
require("dotenv").config();

class MistralService {
  constructor() {
    this.apiKey = process.env.MISTRAL_API_KEY;

    if (!this.apiKey) {
      const errorMsg =
        "MISTRAL_API_KEY environment variable is not set or is invalid. Please set a valid Mistral API key.";
      logger.error(errorMsg);
      throw new Error(errorMsg);
    }

    this.baseURL = "https://api.mistral.ai/v1";
    this.model = process.env.MISTRAL_MODEL || "mistral-large-latest";

    this.client = axios.create({
      baseURL: this.baseURL,
      headers: {
        Authorization: `Bearer ${this.apiKey}`,
        "Content-Type": "application/json",
        Accept: "application/json",
      },
      timeout: 60000,
    });

    logger.info("Mistral service initialized with direct API calls", {
      model: this.model,
      baseURL: this.baseURL,
      apiKeyPresent: !!this.apiKey,
      apiKeyPrefix: this.apiKey
        ? this.apiKey.substring(0, 8) + "..."
        : "not set",
    });
  }

  async makeRequestWithRetry(requestFn, maxRetries = 3, baseDelay = 1000) {
    for (let attempt = 1; attempt <= maxRetries; attempt++) {
      try {
        return await requestFn();
      } catch (error) {
        // Check if it's a rate limit error (429)
        if (error.response?.status === 429) {
          if (attempt === maxRetries) {
            logger.error("Max retries reached for rate-limited request", {
              attempts: maxRetries,
              finalError: error.message,
            });
            throw new Error(
              `Rate limit exceeded after ${maxRetries} attempts: ${error.message}`
            );
          }

          // Extract retry-after header if available
          const retryAfter = error.response?.headers["retry-after"];
          const delay = retryAfter
            ? parseInt(retryAfter) * 1000 // Convert seconds to milliseconds
            : baseDelay * Math.pow(2, attempt - 1); // Exponential backoff

          logger.warn("Rate limit hit, retrying after delay", {
            attempt,
            maxRetries,
            delay,
            retryAfter,
            status: error.response?.status,
          });

          await new Promise((resolve) => setTimeout(resolve, delay));
          continue;
        }

        // For non-rate-limit errors, throw immediately
        throw error;
      }
    }
  }

  async analyzCV(cvData, userPreferences = {}) {
    try {
      const startTime = Date.now();

      const prompt = this.createCVAnalysisPrompt(cvData, userPreferences);

      const requestBody = {
        model: this.model,
        messages: [
          {
            role: "system",
            content: this.getSystemPrompt(),
          },
          {
            role: "user",
            content: prompt,
          },
        ],
        temperature: 0.3,
        max_tokens: 4000,
      };

      logger.info("Making CV analysis request to Mistral API", {
        model: this.model,
        promptLength: prompt.length,
        hasContact: !!cvData.contact,
        hasExperience: !!(cvData.experience && cvData.experience.length > 0),
        hasEducation: !!(cvData.education && cvData.education.length > 0),
        hasProjects: !!(cvData.projects && cvData.projects.length > 0),
        hasSkills: !!(cvData.skills && cvData.skills.length > 0),
        targetIndustry: userPreferences.targetIndustry,
        targetRole: userPreferences.targetRole,
      });

      const response = await this.makeRequestWithRetry(async () => {
        return await this.client.post("/chat/completions", requestBody);
      });

      const processingTime = Date.now() - startTime;

      const analysis = this.parseAnalysisResponse(
        response.data.choices[0].message.content
      );

      logger.info("CV analysis completed", {
        processingTime,
        overallScore: analysis.overallScore,
        tokensUsed: response.data.usage,
      });

      return {
        analysis,
        processingTime,
        rawResponse: response.data,
      };
    } catch (error) {
      logger.error("Mistral CV analysis failed:", {
        error: error.message,
        status: error.response?.status,
        statusText: error.response?.statusText,
        data: error.response?.data,
      });
      throw new Error(`CV analysis failed: ${error.message}`);
    }
  }

  async chatQA(question, conversationHistory = [], cvContext = null) {
    try {
      const messages = [
        {
          role: "system",
          content: this.getQASystemPrompt(cvContext),
        },
      ];

      conversationHistory.forEach((msg) => {
        messages.push({
          role: msg.role,
          content: msg.message,
        });
      });

      messages.push({
        role: "user",
        content: question,
      });

      const requestBody = {
        model: this.model,
        messages,
        temperature: 0.5,
        max_tokens: 1000,
      };

      logger.info("Making Q&A request to Mistral API", {
        model: this.model,
        questionLength: question.length,
        historyLength: conversationHistory.length,
      });

      const response = await this.makeRequestWithRetry(async () => {
        return await this.client.post("/chat/completions", requestBody);
      });

      return {
        response: response.data.choices[0].message.content,
        usage: response.data.usage,
      };
    } catch (error) {
      logger.error("Mistral Q&A failed:", {
        error: error.message,
        status: error.response?.status,
        statusText: error.response?.statusText,
        data: error.response?.data,
      });
      throw new Error(`Q&A failed: ${error.message}`);
    }
  }

  async generateImprovementSuggestions(
    cvContent,
    targetSection,
    specificIssues = []
  ) {
    try {
      const prompt = this.createImprovementPrompt(
        cvContent,
        targetSection,
        specificIssues
      );

      const requestBody = {
        model: this.model,
        messages: [
          {
            role: "system",
            content:
              "You are an expert CV improvement specialist. Provide specific, actionable suggestions.",
          },
          {
            role: "user",
            content: prompt,
          },
        ],
        temperature: 0.4,
        max_tokens: 2000,
      };

      logger.info("Making improvement suggestions request to Mistral API", {
        model: this.model,
        targetSection,
        issuesCount: specificIssues.length,
      });

      const response = await this.makeRequestWithRetry(async () => {
        return await this.client.post("/chat/completions", requestBody);
      });

      return this.parseImprovementSuggestions(
        response.data.choices[0].message.content
      );
    } catch (error) {
      logger.error("Improvement suggestions generation failed:", {
        error: error.message,
        status: error.response?.status,
        statusText: error.response?.statusText,
        data: error.response?.data,
      });
      throw new Error(`Improvement suggestions failed: ${error.message}`);
    }
  }

  getSystemPrompt() {
    return `You are an expert CV/resume analyst with deep knowledge of hiring practices across industries.

Your task is to analyze CVs and provide detailed, constructive feedback. You should:
1. Evaluate each section objectively (0-100 score)
2. Identify strengths and weaknesses
3. Provide specific, actionable improvement suggestions
4. Consider ATS (Applicant Tracking System) compatibility
5. Offer industry-specific advice when relevant

Always respond in valid JSON format with the exact structure requested.
Be professional, constructive, and specific in your feedback.
Focus on both content quality and presentation effectiveness.`;
  }

  getQASystemPrompt(cvContext) {
    let prompt = `You are a helpful CV writing assistant. Answer questions about resume writing, job applications, and career development.

Provide practical, actionable advice. Be encouraging but honest.
Keep responses concise but comprehensive.`;

    if (cvContext) {
      prompt += `\n\nCV Context: ${cvContext}`;
    }

    return prompt;
  }

  createCVAnalysisPrompt(cvData, preferences) {
    const {
      targetIndustry,
      targetRole,
      experienceLevel,
      focusAreas = [],
    } = preferences;

    let cvContent = "";

    if (cvData.title) {
      cvContent += `CV Title: ${cvData.title}\n\n`;
    }

    if (cvData.contact) {
      cvContent += "CONTACT INFORMATION:\n";
      if (cvData.contact.email) cvContent += `Email: ${cvData.contact.email}\n`;
      if (cvData.contact.phoneNumber)
        cvContent += `Phone: ${cvData.contact.phoneNumber}\n`;
      if (cvData.contact.linkedInUrl)
        cvContent += `LinkedIn: ${cvData.contact.linkedInUrl}\n`;
      if (cvData.contact.githubUrl)
        cvContent += `GitHub: ${cvData.contact.githubUrl}\n`;
      if (cvData.contact.personalWebsiteUrl)
        cvContent += `Website: ${cvData.contact.personalWebsiteUrl}\n`;
      if (cvData.contact.city && cvData.contact.country) {
        cvContent += `Location: ${cvData.contact.city}, ${cvData.contact.country}\n`;
      }
      cvContent += "\n";
    }

    if (cvData.summary && cvData.summary.context) {
      cvContent += "PROFESSIONAL SUMMARY:\n";
      cvContent += `${cvData.summary.context}\n\n`;
    }

    if (cvData.skills && cvData.skills.length > 0) {
      cvContent += "SKILLS:\n";
      cvData.skills.forEach((skillGroup) => {
        if (skillGroup.category && skillGroup.items) {
          cvContent += `${skillGroup.category}:\n`;
          cvContent += `  ${skillGroup.items.join(", ")}\n`;
        }
      });
      cvContent += "\n";
    }

    if (cvData.experience && cvData.experience.length > 0) {
      cvContent += "PROFESSIONAL EXPERIENCE:\n";
      cvData.experience.forEach((exp, index) => {
        cvContent += `${index + 1}. ${exp.jobTitle || "Position"} at ${
          exp.company || "Company"
        }\n`;
        if (exp.location) cvContent += `   Location: ${exp.location}\n`;
        if (exp.startDate || exp.endDate) {
          const start = exp.startDate
            ? new Date(exp.startDate).toLocaleDateString()
            : "N/A";
          const end = exp.endDate
            ? new Date(exp.endDate).toLocaleDateString()
            : "Present";
          cvContent += `   Duration: ${start} - ${end}\n`;
        }
        if (exp.description) cvContent += `   ${exp.description}\n`;
        cvContent += "\n";
      });
    }

    if (cvData.education && cvData.education.length > 0) {
      cvContent += "EDUCATION:\n";
      cvData.education.forEach((edu, index) => {
        cvContent += `${index + 1}. ${edu.degree || "Degree"} in ${
          edu.fieldOfStudy || "Field"
        }\n`;
        cvContent += `   Institution: ${edu.organization || "Institution"}\n`;
        if (edu.startDate || edu.endDate) {
          const start = edu.startDate
            ? new Date(edu.startDate).getFullYear()
            : "N/A";
          const end = edu.endDate
            ? new Date(edu.endDate).getFullYear()
            : "Present";
          cvContent += `   Period: ${start} - ${end}\n`;
        }
        if (edu.gpa) cvContent += `   GPA: ${edu.gpa}\n`;
        if (edu.description) cvContent += `   ${edu.description}\n`;
        cvContent += "\n";
      });
    }

    if (cvData.projects && cvData.projects.length > 0) {
      cvContent += "PROJECTS:\n";
      cvData.projects.forEach((project, index) => {
        cvContent += `${index + 1}. ${project.title || "Project"}\n`;
        if (project.description)
          cvContent += `   Description: ${project.description}\n`;
        if (project.link) cvContent += `   Link: ${project.link}\n`;
        if (project.startDate || project.endDate) {
          const start = project.startDate
            ? new Date(project.startDate).toLocaleDateString()
            : "N/A";
          const end = project.endDate
            ? new Date(project.endDate).toLocaleDateString()
            : "Ongoing";
          cvContent += `   Duration: ${start} - ${end}\n`;
        }
        cvContent += "\n";
      });
    }

    if (cvData.certifications && cvData.certifications.length > 0) {
      cvContent += "CERTIFICATIONS:\n";
      cvData.certifications.forEach((cert, index) => {
        cvContent += `${index + 1}. ${cert.name}\n`;
        if (cert.organization) cvContent += `   Issuer: ${cert.organization}\n`;
        if (cert.issuedDate)
          cvContent += `   Issued: ${new Date(
            cert.issuedDate
          ).toLocaleDateString()}\n`;
        if (cert.relevance) cvContent += `   Relevance: ${cert.relevance}\n`;
        cvContent += "\n";
      });
    }

    if (cvData.template) {
      cvContent += "TEMPLATE INFORMATION:\n";
      cvContent += `Template: ${cvData.template.name || "Custom"}\n\n`;
    }

    return `Please analyze the following CV and provide detailed, constructive feedback:

CV CONTENT:
${cvContent}

ANALYSIS CONTEXT:
- Target Industry: ${targetIndustry || "General"}
- Target Role: ${targetRole || "Not specified"}
- Experience Level: ${experienceLevel || "Not specified"}
- Focus Areas: ${
      focusAreas.length > 0 ? focusAreas.join(", ") : "General improvement"
    }

Please provide a comprehensive analysis with specific, actionable feedback. Respond with a JSON object containing:

{
  "overallScore": <number 0-100>,
  "sectionAnalysis": {
    "summary": {
      "score": <number 0-100>,
      "feedback": "<detailed feedback>",
      "suggestions": ["<suggestion1>", "<suggestion2>"],
      "strengths": ["<strength1>", "<strength2>"],
      "weaknesses": ["<weakness1>", "<weakness2>"]
    },
    "skills": {
      "score": <number 0-100>,
      "feedback": "<detailed feedback>",
      "suggestions": ["<suggestion1>", "<suggestion2>"],
      "strengths": ["<strength1>", "<strength2>"],
      "weaknesses": ["<weakness1>", "<weakness2>"],
      "missingSkills": ["<skill1>", "<skill2>"],
      "relevantSkills": ["<skill1>", "<skill2>"]
    },
    "experience": {
      "score": <number 0-100>,
      "feedback": "<detailed feedback>",
      "suggestions": ["<suggestion1>", "<suggestion2>"],
      "strengths": ["<strength1>", "<strength2>"],
      "weaknesses": ["<weakness1>", "<weakness2>"],
      "careerProgression": "<assessment>",
      "quantificationLevel": "<assessment>"
    },
    "education": {
      "score": <number 0-100>,
      "feedback": "<detailed feedback>",
      "suggestions": ["<suggestion1>", "<suggestion2>"],
      "strengths": ["<strength1>", "<strength2>"],
      "weaknesses": ["<weakness1>", "<weakness2>"],
      "relevance": "<assessment>"
    },
    "projects": {
      "score": <number 0-100>,
      "feedback": "<detailed feedback>",
      "suggestions": ["<suggestion1>", "<suggestion2>"],
      "strengths": ["<strength1>", "<strength2>"],
      "weaknesses": ["<weakness1>", "<weakness2>"],
      "technicalDepth": "<assessment>",
      "businessImpact": "<assessment>"
    },
    "certifications": {
      "score": <number 0-100>,
      "feedback": "<detailed feedback>",
      "suggestions": ["<suggestion1>", "<suggestion2>"],
      "strengths": ["<strength1>", "<strength2>"],
      "weaknesses": ["<weakness1>", "<weakness2>"],
      "industryRelevance": "<assessment>"
    },
    "formatting": {
      "score": <number 0-100>,
      "feedback": "<detailed feedback>",
      "suggestions": ["<suggestion1>", "<suggestion2>"],
      "strengths": ["<strength1>", "<strength2>"],
      "weaknesses": ["<weakness1>", "<weakness2>"]
    }
  },
  "improvementSuggestions": [
    {
      "id": "<unique_id>",
      "section": "<summary|skills|experience|education|projects|certifications|formatting|general>",
      "priority": "<low|medium|high|critical>",
      "suggestion": "<specific suggestion>",
      "explanation": "<why this matters>",
      "autoApplicable": <boolean>,
      "category": "<content|structure|technical|formatting|keywords>",
      "estimatedImpact": "<low|medium|high>"
    }
  ],
  "industrySpecificAdvice": {
    "targetIndustry": "${targetIndustry || "General"}",
    "relevanceScore": <number 0-100>,
    "industryTrends": ["<trend1>", "<trend2>"],
    "recommendedSkills": ["<skill1>", "<skill2>"],
    "industrySpecificSuggestions": ["<suggestion1>", "<suggestion2>"],
    "competitorAnalysis": "<insights>",
    "marketDemand": "<assessment>"
  },
  "atsCompatibility": {
    "score": <number 0-100>,
    "issues": ["<issue1>", "<issue2>"],
    "recommendations": ["<recommendation1>", "<recommendation2>"],
    "keywordDensity": <number>,
    "formatCompliance": "<assessment>",
    "scanability": "<assessment>"
  }
}

Focus on providing specific, actionable feedback that helps improve the CV's effectiveness for the target industry and role.`;
  }

  createImprovementPrompt(cvContent, targetSection, specificIssues) {
    return `Please provide specific improvement suggestions for the ${targetSection} section of this CV:

CV CONTENT:
${cvContent}

SPECIFIC ISSUES TO ADDRESS:
${specificIssues.join(", ")}

Provide 3-5 specific, actionable suggestions with before/after examples where possible.`;
  }

  parseAnalysisResponse(responseText) {
    try {
      const jsonMatch = responseText.match(/\{[\s\S]*\}/);
      if (!jsonMatch) {
        throw new Error("No valid JSON found in response");
      }

      const parsedResponse = JSON.parse(jsonMatch[0]);

      this.validateAnalysisResponse(parsedResponse);

      return parsedResponse;
    } catch (error) {
      logger.error("Failed to parse Mistral response:", error);

      return this.getFallbackAnalysis(responseText);
    }
  }

  parseImprovementSuggestions(responseText) {
    try {
      const suggestions = [];
      const lines = responseText.split("\n");

      lines.forEach((line) => {
        if (line.trim().startsWith("-") || line.trim().startsWith("•")) {
          suggestions.push(line.trim().substring(1).trim());
        }
      });

      return suggestions;
    } catch (error) {
      logger.error("Failed to parse improvement suggestions:", error);
      return ["Unable to parse suggestions. Please try again."];
    }
  }

  validateAnalysisResponse(response) {
    const requiredFields = [
      "overallScore",
      "sectionAnalysis",
      "improvementSuggestions",
    ];

    for (const field of requiredFields) {
      if (!response[field]) {
        throw new Error(`Missing required field: ${field}`);
      }
    }

    if (
      typeof response.overallScore !== "number" ||
      response.overallScore < 0 ||
      response.overallScore > 100
    ) {
      throw new Error("Invalid overall score");
    }
  }

  getFallbackAnalysis(originalResponse) {
    return {
      overallScore: 50,
      sectionAnalysis: {
        careerObjective: {
          score: 50,
          feedback: "Unable to analyze - please try again",
          suggestions: [],
          strengths: [],
          weaknesses: [],
        },
        skills: {
          score: 50,
          feedback: "Unable to analyze - please try again",
          suggestions: [],
          strengths: [],
          weaknesses: [],
          missingSkills: [],
          industryRelevance: "Unable to assess",
        },
        experience: {
          score: 50,
          feedback: "Unable to analyze - please try again",
          suggestions: [],
          strengths: [],
          weaknesses: [],
          quantificationNeeded: [],
        },
        education: {
          score: 50,
          feedback: "Unable to analyze - please try again",
          suggestions: [],
          strengths: [],
          weaknesses: [],
        },
        projects: {
          score: 50,
          feedback: "Unable to analyze - please try again",
          suggestions: [],
          strengths: [],
          weaknesses: [],
        },
        formatting: {
          score: 50,
          feedback: "Unable to analyze - please try again",
          suggestions: [],
          issues: [],
        },
      },
      improvementSuggestions: [],
      industrySpecificAdvice: {
        targetIndustry: "General",
        relevanceScore: 50,
        industryTrends: [],
        recommendedSkills: [],
        industrySpecificSuggestions: [],
      },
      atsCompatibility: {
        score: 50,
        issues: [],
        recommendations: [],
      },
      rawResponse: originalResponse,
    };
  }
}

module.exports = new MistralService();
