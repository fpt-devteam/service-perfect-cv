const logger = require("../config/logger");

// Request logging middleware
const requestLogger = (req, res, next) => {
  const startTime = Date.now();

  // Skip logging for health checks and static assets
  const skipPaths = ["/health", "/favicon.ico"];

  if (skipPaths.includes(req.path)) {
    return next();
  }

  // Capture response data
  const originalSend = res.send;
  const originalJson = res.json;

  let responseBody = null;
  let responseSize = 0;

  // Override res.send to capture response
  res.send = function (body) {
    responseBody = body;
    responseSize = Buffer.byteLength(body || "", "utf8");
    return originalSend.call(this, body);
  };

  // Override res.json to capture response
  res.json = function (obj) {
    responseBody = obj;
    responseSize = Buffer.byteLength(JSON.stringify(obj || {}), "utf8");
    return originalJson.call(this, obj);
  };

  // Log request details
  const requestData = {
    method: req.method,
    url: req.originalUrl,
    path: req.path,
    ip: req.ip || req.connection.remoteAddress,
    userAgent: req.get("User-Agent"),
    referer: req.get("Referer"),
    contentType: req.get("Content-Type"),
    contentLength: req.get("Content-Length"),
    query: Object.keys(req.query).length > 0 ? req.query : undefined,
    params: Object.keys(req.params).length > 0 ? req.params : undefined,
    timestamp: new Date().toISOString(),
    requestId: req.id || generateRequestId(),
  };

  // Add request body for non-GET requests (but sanitize sensitive data)
  if (req.method !== "GET" && req.body) {
    requestData.body = sanitizeRequestBody(req.body);
  }

  // Log the incoming request
  logger.info("Incoming request", requestData);

  // Capture response when request finishes
  res.on("finish", () => {
    const duration = Date.now() - startTime;

    const responseData = {
      requestId: requestData.requestId,
      method: req.method,
      url: req.originalUrl,
      statusCode: res.statusCode,
      duration: `${duration}ms`,
      responseSize: `${responseSize} bytes`,
      ip: requestData.ip,
      userAgent: requestData.userAgent,
      timestamp: new Date().toISOString(),
    };

    // Log response based on status code
    if (res.statusCode >= 500) {
      logger.error("Request completed with server error", {
        ...responseData,
        responseBody:
          process.env.NODE_ENV === "development" ? responseBody : undefined,
      });
    } else if (res.statusCode >= 400) {
      logger.warn("Request completed with client error", {
        ...responseData,
        responseBody:
          process.env.NODE_ENV === "development" ? responseBody : undefined,
      });
    } else {
      logger.info("Request completed successfully", responseData);
    }

    // Log slow requests (> 1 second)
    if (duration > 1000) {
      logger.warn("Slow request detected", {
        ...responseData,
        warning: "Request took longer than 1 second",
      });
    }
  });

  // Handle request errors
  res.on("error", (error) => {
    logger.error("Request error", {
      requestId: requestData.requestId,
      method: req.method,
      url: req.originalUrl,
      error: error.message,
      stack: error.stack,
      ip: requestData.ip,
    });
  });

  next();
};

// Generate unique request ID
const generateRequestId = () => {
  return `req_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
};

// Sanitize request body to remove sensitive information
const sanitizeRequestBody = (body) => {
  if (!body || typeof body !== "object") {
    return body;
  }

  const sensitiveFields = [
    "password",
    "token",
    "api_key",
    "apiKey",
    "secret",
    "auth",
    "authorization",
    "credit_card",
    "creditCard",
    "ssn",
    "social_security_number",
  ];

  const sanitized = { ...body };

  // Recursively sanitize nested objects
  const sanitizeObject = (obj) => {
    for (const key in obj) {
      if (obj.hasOwnProperty(key)) {
        const lowerKey = key.toLowerCase();

        // Check if key contains sensitive information
        const isSensitive = sensitiveFields.some((field) =>
          lowerKey.includes(field.toLowerCase())
        );

        if (isSensitive) {
          obj[key] = "[REDACTED]";
        } else if (typeof obj[key] === "object" && obj[key] !== null) {
          sanitizeObject(obj[key]);
        } else if (typeof obj[key] === "string" && obj[key].length > 1000) {
          // Truncate very long strings (like CV content)
          obj[key] = obj[key].substring(0, 100) + "... [TRUNCATED]";
        }
      }
    }
  };

  sanitizeObject(sanitized);
  return sanitized;
};

// Enhanced logging for specific routes
const enhancedLogger = (options = {}) => {
  return (req, res, next) => {
    const {
      logBody = false,
      logResponse = false,
      logHeaders = false,
    } = options;

    if (logHeaders) {
      logger.debug("Request headers", {
        url: req.originalUrl,
        headers: req.headers,
      });
    }

    if (logBody && req.body) {
      logger.debug("Request body", {
        url: req.originalUrl,
        body: sanitizeRequestBody(req.body),
      });
    }

    if (logResponse) {
      const originalJson = res.json;
      res.json = function (obj) {
        logger.debug("Response body", {
          url: req.originalUrl,
          statusCode: res.statusCode,
          response: obj,
        });
        return originalJson.call(this, obj);
      };
    }

    next();
  };
};

module.exports = {
  requestLogger,
  enhancedLogger,
  sanitizeRequestBody,
};
