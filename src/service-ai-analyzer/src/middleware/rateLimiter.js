const rateLimit = require("express-rate-limit");
const logger = require("../config/logger");

// Global rate limiter - applies to all requests
const globalRateLimiter = rateLimit({
  windowMs: parseInt(process.env.RATE_LIMIT_WINDOW_MS) || 15 * 60 * 1000, // 15 minutes
  max: parseInt(process.env.RATE_LIMIT_MAX_REQUESTS) || 1000, // limit each IP to 1000 requests per windowMs
  message: {
    success: false,
    message: "Too many requests from this IP, please try again later.",
    retryAfter: "15 minutes",
  },
  standardHeaders: true, // Return rate limit info in the `RateLimit-*` headers
  legacyHeaders: false, // Disable the `X-RateLimit-*` headers
  handler: (req, res) => {
    logger.warn("Rate limit exceeded", {
      ip: req.ip,
      userAgent: req.get("User-Agent"),
      path: req.path,
      method: req.method,
    });

    res.status(429).json({
      success: false,
      message: "Too many requests from this IP, please try again later.",
      retryAfter: Math.ceil((req.rateLimit.resetTime - Date.now()) / 1000),
    });
  },
  skip: (req) => {
    // Skip rate limiting for health checks
    return req.path === "/health";
  },
});

// Flexible rate limiter factory
const rateLimiter = (maxRequests = 100, windowMinutes = 15) => {
  return rateLimit({
    windowMs: windowMinutes * 60 * 1000,
    max: maxRequests,
    message: {
      success: false,
      message: `Too many requests. Limit: ${maxRequests} requests per ${windowMinutes} minutes.`,
      retryAfter: `${windowMinutes} minutes`,
    },
    standardHeaders: true,
    legacyHeaders: false,
    handler: (req, res) => {
      logger.warn("Endpoint rate limit exceeded", {
        ip: req.ip,
        path: req.path,
        method: req.method,
        limit: maxRequests,
        window: windowMinutes,
      });

      res.status(429).json({
        success: false,
        message: `Too many requests. Limit: ${maxRequests} requests per ${windowMinutes} minutes.`,
        retryAfter: Math.ceil((req.rateLimit.resetTime - Date.now()) / 1000),
      });
    },
  });
};

// Specific rate limiters for different endpoints
const cvAnalysisLimiter = rateLimiter(5, 60); // 5 CV analyses per hour
const qaLimiter = rateLimiter(50, 15); // 50 questions per 15 minutes
const suggestionLimiter = rateLimiter(100, 60); // 100 suggestion applications per hour

module.exports = {
  globalRateLimiter,
  rateLimiter,
  cvAnalysisLimiter,
  qaLimiter,
  suggestionLimiter,
};
