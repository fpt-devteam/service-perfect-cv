const Joi = require("joi");
const { AppError } = require("./errorHandler");

// Common validation schemas
const schemas = {
  // MongoDB ObjectId validation
  objectId: Joi.string()
    .pattern(/^[0-9a-fA-F]{24}$/)
    .required()
    .messages({
      "string.pattern.base": "Invalid ID format",
      "any.required": "ID is required",
    }),

  // UUID validation for main system IDs
  uuid: Joi.string().uuid().required().messages({
    "string.guid": "Invalid UUID format",
    "any.required": "UUID is required",
  }),

  // User ID validation (UUID from main system)
  userId: Joi.string().uuid().required().messages({
    "string.guid": "Invalid user ID format",
    "any.required": "User ID is required",
  }),

  // CV ID validation (UUID from main system)
  cvId: Joi.string().uuid().required().messages({
    "string.guid": "Invalid CV ID format",
    "any.required": "CV ID is required",
  }),

  // Analysis ID validation (MongoDB ObjectId for our analysis records)
  analysisId: Joi.string()
    .pattern(/^[0-9a-fA-F]{24}$/)
    .required()
    .messages({
      "string.pattern.base": "Invalid analysis ID format",
      "any.required": "Analysis ID is required",
    }),

  // Session ID validation (UUID format)
  sessionId: Joi.string().uuid().required().messages({
    "string.guid": "Invalid session ID format",
    "any.required": "Session ID is required",
  }),

  // Contact information schema
  contact: Joi.object({
    phoneNumber: Joi.string().max(20).optional(),
    email: Joi.string().email().optional(),
    linkedInUrl: Joi.string().uri().optional(),
    githubUrl: Joi.string().uri().optional(),
    personalWebsiteUrl: Joi.string().uri().optional(),
    country: Joi.string().max(100).optional(),
    city: Joi.string().max(100).optional(),
  }).optional(),

  // Summary schema
  summary: Joi.object({
    context: Joi.string().max(2000).optional(),
  }).optional(),

  // Education item schema
  education: Joi.object({
    degree: Joi.string().max(200).optional(),
    degreeId: Joi.string().uuid().optional(),
    organization: Joi.string().max(200).optional(),
    organizationId: Joi.string().uuid().optional(),
    fieldOfStudy: Joi.string().max(200).optional(),
    startDate: Joi.date().optional(),
    endDate: Joi.date().optional(),
    gpa: Joi.number().min(0).max(10).optional(),
    description: Joi.string().max(1000).optional(),
  }),

  // Experience item schema
  experience: Joi.object({
    jobTitle: Joi.string().max(200).optional(),
    jobTitleId: Joi.string().uuid().optional(),
    employmentTypeId: Joi.string().uuid().optional(),
    company: Joi.string().max(200).optional(),
    companyId: Joi.string().uuid().optional(),
    location: Joi.string().max(200).optional(),
    startDate: Joi.date().optional(),
    endDate: Joi.date().optional(),
    description: Joi.string().max(2000).optional(),
  }),

  // Project item schema
  project: Joi.object({
    title: Joi.string().max(200).optional(),
    description: Joi.string().max(2000).optional(),
    link: Joi.string().uri().optional(),
    startDate: Joi.date().optional(),
    endDate: Joi.date().optional(),
  }),

  // Skills item schema
  skill: Joi.object({
    category: Joi.string().max(100).optional(),
    items: Joi.array().items(Joi.string().max(100)).optional(),
  }),

  // Certification item schema
  certification: Joi.object({
    name: Joi.string().max(200).optional(),
    organization: Joi.string().max(200).optional(),
    organizationId: Joi.string().uuid().optional(),
    issuedDate: Joi.date().optional(),
    relevance: Joi.string().max(500).optional(),
  }),

  // Template schema
  template: Joi.object({
    id: Joi.string().uuid().optional(),
    name: Joi.string().max(200).optional(),
    descriptor: Joi.object().optional(),
    cssUrl: Joi.string().uri().optional(),
    reactBundle: Joi.string().optional(),
    previewUrl: Joi.string().uri().optional(),
  }).optional(),

  // CV Analysis submission validation
  cvAnalysisSubmission: Joi.object({
    cvId: Joi.string().uuid().required().messages({
      "string.guid": "Invalid CV ID format",
      "any.required": "CV ID is required",
    }),
    userId: Joi.string().uuid().required().messages({
      "string.guid": "Invalid user ID format",
      "any.required": "User ID is required",
    }),
    cvData: Joi.object({
      title: Joi.string().max(200).optional(),
      contact: Joi.object({
        phoneNumber: Joi.string().max(20).optional(),
        email: Joi.string().email().optional(),
        linkedInUrl: Joi.string().uri().optional(),
        githubUrl: Joi.string().uri().optional(),
        personalWebsiteUrl: Joi.string().uri().optional(),
        country: Joi.string().max(100).optional(),
        city: Joi.string().max(100).optional(),
      }).optional(),
      summary: Joi.object({
        context: Joi.string().max(2000).optional(),
      }).optional(),
      education: Joi.array()
        .items(
          Joi.object({
            degree: Joi.string().max(200).optional(),
            degreeId: Joi.string().uuid().optional(),
            organization: Joi.string().max(200).optional(),
            organizationId: Joi.string().uuid().optional(),
            fieldOfStudy: Joi.string().max(200).optional(),
            startDate: Joi.date().optional(),
            endDate: Joi.date().optional(),
            gpa: Joi.number().min(0).max(10).optional(),
            description: Joi.string().max(1000).optional(),
          })
        )
        .optional(),
      experience: Joi.array()
        .items(
          Joi.object({
            jobTitle: Joi.string().max(200).optional(),
            jobTitleId: Joi.string().uuid().optional(),
            employmentTypeId: Joi.string().uuid().optional(),
            company: Joi.string().max(200).optional(),
            companyId: Joi.string().uuid().optional(),
            location: Joi.string().max(200).optional(),
            startDate: Joi.date().optional(),
            endDate: Joi.date().optional(),
            description: Joi.string().max(2000).optional(),
          })
        )
        .optional(),
      projects: Joi.array()
        .items(
          Joi.object({
            title: Joi.string().max(200).optional(),
            description: Joi.string().max(2000).optional(),
            link: Joi.string().uri().optional(),
            startDate: Joi.date().optional(),
            endDate: Joi.date().optional(),
          })
        )
        .optional(),
      skills: Joi.array()
        .items(
          Joi.object({
            category: Joi.string().max(100).optional(),
            items: Joi.array().items(Joi.string().max(100)).optional(),
          })
        )
        .optional(),
      certifications: Joi.array()
        .items(
          Joi.object({
            name: Joi.string().max(200).optional(),
            organization: Joi.string().max(200).optional(),
            organizationId: Joi.string().uuid().optional(),
            issuedDate: Joi.date().optional(),
            relevance: Joi.string().max(500).optional(),
          })
        )
        .optional(),
      template: Joi.object({
        id: Joi.string().uuid().optional(),
        name: Joi.string().max(200).optional(),
        descriptor: Joi.object().optional(),
        cssUrl: Joi.string().uri().optional(),
        reactBundle: Joi.string().optional(),
        previewUrl: Joi.string().uri().optional(),
      }).optional(),
    })
      .required()
      .messages({
        "any.required": "CV data is required",
      }),
    userPreferences: Joi.object({
      targetIndustry: Joi.string().max(100).optional(),
      targetRole: Joi.string().max(100).optional(),
      experienceLevel: Joi.string()
        .valid("entry", "mid", "senior", "executive")
        .optional(),
      focusAreas: Joi.array()
        .items(
          Joi.string().valid(
            "technical-skills",
            "leadership",
            "achievements",
            "education",
            "certifications",
            "projects"
          )
        )
        .optional(),
      urgent: Joi.boolean().default(false),
    })
      .optional()
      .default({}),
    requestMetadata: Joi.object({
      source: Joi.string().max(100).optional(),
      version: Joi.string().max(50).optional(),
      submittedAt: Joi.date().optional(),
    })
      .optional()
      .default({}),
  }),

  // Q&A question validation
  question: Joi.object({
    userId: Joi.string().uuid().required(),
    question: Joi.string().min(5).max(2000).required().messages({
      "string.min": "Question must be at least 5 characters",
      "string.max": "Question cannot exceed 2,000 characters",
      "any.required": "Question is required",
    }),
    sessionId: Joi.string().uuid().allow(null).optional(),
    cvId: Joi.string().uuid().allow(null).optional(),
  }),

  // Suggestion application validation
  suggestionApplication: Joi.object({
    suggestionId: Joi.string().required().messages({
      "any.required": "Suggestion ID is required",
    }),
    appliedBy: Joi.string().max(100).optional(),
  }),

  // CV reanalysis validation
  reanalysis: Joi.object({
    updatedCvData: Joi.object({
      title: Joi.string().max(200).optional(),
      contact: Joi.object({
        phoneNumber: Joi.string().max(20).optional(),
        email: Joi.string().email().optional(),
        linkedInUrl: Joi.string().uri().optional(),
        githubUrl: Joi.string().uri().optional(),
        personalWebsiteUrl: Joi.string().uri().optional(),
        country: Joi.string().max(100).optional(),
        city: Joi.string().max(100).optional(),
      }).optional(),
      summary: Joi.object({
        context: Joi.string().max(2000).optional(),
      }).optional(),
      education: Joi.array()
        .items(
          Joi.object({
            degree: Joi.string().max(200).optional(),
            degreeId: Joi.string().uuid().optional(),
            organization: Joi.string().max(200).optional(),
            organizationId: Joi.string().uuid().optional(),
            fieldOfStudy: Joi.string().max(200).optional(),
            startDate: Joi.date().optional(),
            endDate: Joi.date().optional(),
            gpa: Joi.number().min(0).max(10).optional(),
            description: Joi.string().max(1000).optional(),
          })
        )
        .optional(),
      experience: Joi.array()
        .items(
          Joi.object({
            jobTitle: Joi.string().max(200).optional(),
            jobTitleId: Joi.string().uuid().optional(),
            employmentTypeId: Joi.string().uuid().optional(),
            company: Joi.string().max(200).optional(),
            companyId: Joi.string().uuid().optional(),
            location: Joi.string().max(200).optional(),
            startDate: Joi.date().optional(),
            endDate: Joi.date().optional(),
            description: Joi.string().max(2000).optional(),
          })
        )
        .optional(),
      projects: Joi.array()
        .items(
          Joi.object({
            title: Joi.string().max(200).optional(),
            description: Joi.string().max(2000).optional(),
            link: Joi.string().uri().optional(),
            startDate: Joi.date().optional(),
            endDate: Joi.date().optional(),
          })
        )
        .optional(),
      skills: Joi.array()
        .items(
          Joi.object({
            category: Joi.string().max(100).optional(),
            items: Joi.array().items(Joi.string().max(100)).optional(),
          })
        )
        .optional(),
      certifications: Joi.array()
        .items(
          Joi.object({
            name: Joi.string().max(200).optional(),
            organization: Joi.string().max(200).optional(),
            organizationId: Joi.string().uuid().optional(),
            issuedDate: Joi.date().optional(),
            relevance: Joi.string().max(500).optional(),
          })
        )
        .optional(),
      template: Joi.object({
        id: Joi.string().uuid().optional(),
        name: Joi.string().max(200).optional(),
        descriptor: Joi.object().optional(),
        cssUrl: Joi.string().uri().optional(),
        reactBundle: Joi.string().optional(),
        previewUrl: Joi.string().uri().optional(),
      }).optional(),
    }).optional(),
    userPreferences: Joi.object({
      targetIndustry: Joi.string().max(100).optional(),
      targetRole: Joi.string().max(100).optional(),
      experienceLevel: Joi.string()
        .valid("entry", "mid", "senior", "executive")
        .optional(),
      focusAreas: Joi.array()
        .items(
          Joi.string().valid(
            "technical-skills",
            "leadership",
            "achievements",
            "education",
            "certifications",
            "projects"
          )
        )
        .optional(),
      urgent: Joi.boolean().optional(),
    })
      .optional()
      .default({}),
    reason: Joi.string().max(200).optional(),
  }),

  // Pagination validation
  pagination: Joi.object({
    page: Joi.number().integer().min(1).max(1000).default(1),
    limit: Joi.number().integer().min(1).max(100).default(10),
    status: Joi.string()
      .valid("pending", "processing", "completed", "failed")
      .optional(),
    userId: Joi.string().uuid().optional(),
  }),

  // Query parameters validation
  queryParams: {
    version: Joi.number().integer().min(1).optional(),
    status: Joi.string().valid("active", "closed", "archived").optional(),
    period: Joi.string().valid("7d", "30d", "90d").default("30d"),
    cvId: Joi.string().uuid().optional(),
    userId: Joi.string().uuid().optional(),
    fromVersion: Joi.number().integer().min(1).optional(),
    toVersion: Joi.number().integer().min(1).optional(),
  },
};

// Generic validation middleware factory
const validate = (schema, source = "body") => {
  return (req, res, next) => {
    const data =
      source === "body"
        ? req.body
        : source === "params"
        ? req.params
        : source === "query"
        ? req.query
        : req[source];

    const { error, value } = schema.validate(data, {
      abortEarly: false, // Return all validation errors
      stripUnknown: true, // Remove unknown fields
      convert: true, // Convert types (string to number, etc.)
    });

    if (error) {
      const errorMessage = error.details
        .map((detail) => detail.message)
        .join(", ");
      return next(new AppError(`Validation error: ${errorMessage}`, 400));
    }

    // Replace the original data with validated data
    if (source === "body") req.body = value;
    else if (source === "params") req.params = value;
    else if (source === "query") req.query = value;

    next();
  };
};

// Specific validation middleware
const validateCVAnalysis = validate(schemas.cvAnalysisSubmission, "body");

const validateAnalysisId = validate(
  Joi.object({
    analysisId: schemas.analysisId,
  }),
  "params"
);

const validateCVId = validate(
  Joi.object({
    cvId: schemas.cvId,
  }),
  "params"
);

const validateUserId = validate(
  Joi.object({
    userId: schemas.userId,
  }),
  "params"
);

const validateSessionId = validate(
  Joi.object({
    sessionId: schemas.sessionId,
  }),
  "params"
);

const validateQuestion = validate(schemas.question, "body");

const validateSuggestionApplication = validate(
  schemas.suggestionApplication,
  "body"
);

const validateReanalysis = validate(schemas.reanalysis, "body");

const validatePagination = validate(schemas.pagination, "query");

// Combined validation for multiple sources
const validateMultiple = (validations) => {
  return async (req, res, next) => {
    try {
      for (const { schema, source } of validations) {
        const data =
          source === "body"
            ? req.body
            : source === "params"
            ? req.params
            : source === "query"
            ? req.query
            : req[source];

        const { error, value } = schema.validate(data, {
          abortEarly: false,
          stripUnknown: true,
          convert: true,
        });

        if (error) {
          const errorMessage = error.details
            .map((detail) => detail.message)
            .join(", ");
          return next(
            new AppError(`Validation error in ${source}: ${errorMessage}`, 400)
          );
        }

        // Replace the original data with validated data
        if (source === "body") req.body = value;
        else if (source === "params") req.params = value;
        else if (source === "query") req.query = value;
      }
      next();
    } catch (error) {
      next(new AppError("Validation processing error", 500));
    }
  };
};

// Validation for CV analysis feedback with optional query parameters
const validateAnalysisFeedback = validateMultiple([
  { schema: Joi.object({ analysisId: schemas.analysisId }), source: "params" },
  {
    schema: Joi.object({ version: schemas.queryParams.version }),
    source: "query",
  },
]);

// Validation for CV analysis history with query parameters
const validateCVAnalysisHistory = validateMultiple([
  { schema: Joi.object({ cvId: schemas.cvId }), source: "params" },
  {
    schema: Joi.object({
      userId: schemas.queryParams.userId,
      page: Joi.number().integer().min(1).default(1),
      limit: Joi.number().integer().min(1).max(50).default(10),
    }),
    source: "query",
  },
]);

// Validation for user sessions with query parameters
const validateUserSessions = validateMultiple([
  { schema: Joi.object({ userId: schemas.userId }), source: "params" },
  {
    schema: Joi.object({
      status: schemas.queryParams.status,
      page: Joi.number().integer().min(1).default(1),
      limit: Joi.number().integer().min(1).max(50).default(10),
    }),
    source: "query",
  },
]);

// Validation for analytics endpoints
const validateAnalytics = validateMultiple([
  { schema: Joi.object({ userId: schemas.userId }), source: "params" },
  {
    schema: Joi.object({ period: schemas.queryParams.period }),
    source: "query",
  },
]);

// Validation for version comparison
const validateVersionComparison = validateMultiple([
  { schema: Joi.object({ userId: schemas.userId }), source: "params" },
  {
    schema: Joi.object({
      cvId: schemas.queryParams.cvId.required(),
      fromVersion: schemas.queryParams.fromVersion.required(),
      toVersion: schemas.queryParams.toVersion.required(),
    }),
    source: "query",
  },
]);

// Custom validation for file uploads (if needed in future)
const validateFileUpload = (req, res, next) => {
  if (!req.file && !req.body.content) {
    return next(
      new AppError("Either file upload or content text is required", 400)
    );
  }

  if (req.file) {
    // Validate file type
    const allowedTypes = [
      "text/plain",
      "application/pdf",
      "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
    ];
    if (!allowedTypes.includes(req.file.mimetype)) {
      return next(
        new AppError(
          "Invalid file type. Only PDF, DOCX, and TXT files are allowed",
          400
        )
      );
    }

    // Validate file size (10MB limit)
    if (req.file.size > 10 * 1024 * 1024) {
      return next(
        new AppError("File size too large. Maximum size is 10MB", 400)
      );
    }
  }

  next();
};

// Sanitization helper
const sanitizeInput = (input) => {
  if (typeof input === "string") {
    // Remove potential XSS characters
    return input
      .replace(/<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>/gi, "")
      .replace(/<[^>]*>?/gm, "") // Remove HTML tags
      .trim();
  }
  return input;
};

// Sanitization middleware
const sanitizeRequest = (req, res, next) => {
  if (req.body) {
    req.body = sanitizeObject(req.body);
  }
  if (req.query) {
    req.query = sanitizeObject(req.query);
  }
  if (req.params) {
    req.params = sanitizeObject(req.params);
  }
  next();
};

const sanitizeObject = (obj) => {
  if (obj === null || obj === undefined) return obj;

  if (Array.isArray(obj)) {
    return obj.map((item) => sanitizeObject(item));
  }

  if (typeof obj === "object") {
    const sanitized = {};
    for (const key in obj) {
      if (obj.hasOwnProperty(key)) {
        sanitized[key] = sanitizeObject(obj[key]);
      }
    }
    return sanitized;
  }

  return sanitizeInput(obj);
};

module.exports = {
  // Main validation functions
  validate,
  validateMultiple,

  // Specific validators
  validateCVAnalysis,
  validateAnalysisId,
  validateCVId,
  validateUserId,
  validateSessionId,
  validateQuestion,
  validateSuggestionApplication,
  validateReanalysis,
  validatePagination,
  validateAnalysisFeedback,
  validateCVAnalysisHistory,
  validateUserSessions,
  validateAnalytics,
  validateVersionComparison,
  validateFileUpload,

  // Sanitization
  sanitizeRequest,
  sanitizeInput,

  // Schemas for custom validation
  schemas,
};
