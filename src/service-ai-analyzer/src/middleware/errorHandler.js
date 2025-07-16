const logger = require("../config/logger");

// Custom error class for application errors
class AppError extends Error {
  constructor(message, statusCode = 500, isOperational = true) {
    super(message);
    this.statusCode = statusCode;
    this.isOperational = isOperational;
    this.status = `${statusCode}`.startsWith("4") ? "fail" : "error";

    Error.captureStackTrace(this, this.constructor);
  }
}

// Development error response with full stack trace
const sendErrorDev = (err, res) => {
  res.status(err.statusCode).json({
    success: false,
    status: err.status,
    error: err,
    message: err.message,
    stack: err.stack,
  });
};

// Production error response with minimal information
const sendErrorProd = (err, res) => {
  // Operational, trusted error: send message to client
  if (err.isOperational) {
    res.status(err.statusCode).json({
      success: false,
      status: err.status,
      message: err.message,
    });
  } else {
    // Programming or other unknown error: don't leak error details
    logger.error("Unexpected error:", err);

    res.status(500).json({
      success: false,
      status: "error",
      message: "Something went wrong!",
    });
  }
};

// Handle specific MongoDB/Mongoose errors
const handleCastErrorDB = (err) => {
  const message = `Invalid ${err.path}: ${err.value}`;
  return new AppError(message, 400);
};

const handleDuplicateFieldsDB = (err) => {
  const value = err.errmsg.match(/(["'])(\\?.)*?\1/)[0];
  const message = `Duplicate field value: ${value}. Please use another value!`;
  return new AppError(message, 400);
};

const handleValidationErrorDB = (err) => {
  const errors = Object.values(err.errors).map((el) => el.message);
  const message = `Invalid input data. ${errors.join(". ")}`;
  return new AppError(message, 400);
};

// Handle JWT errors
const handleJWTError = () =>
  new AppError("Invalid token. Please log in again!", 401);

const handleJWTExpiredError = () =>
  new AppError("Your token has expired! Please log in again.", 401);

// Handle Joi validation errors
const handleJoiValidationError = (err) => {
  const messages = err.details.map((detail) => detail.message);
  const message = `Validation error: ${messages.join(". ")}`;
  return new AppError(message, 400);
};

// Handle Redis errors
const handleRedisError = (err) => {
  const message = `Redis connection error: ${err.message}`;
  return new AppError(message, 503);
};

// Handle Mistral AI API errors
const handleMistralError = (err) => {
  if (err.response) {
    const status = err.response.status;
    const message = err.response.data?.message || "Mistral AI service error";
    return new AppError(
      `AI service error: ${message}`,
      status >= 500 ? 503 : 400
    );
  }
  return new AppError("AI service temporarily unavailable", 503);
};

// Main error handling middleware
const errorHandler = (err, req, res, next) => {
  let error = { ...err };
  error.message = err.message;

  // Log error details
  logger.error("Error occurred:", {
    message: err.message,
    stack: err.stack,
    url: req.originalUrl,
    method: req.method,
    ip: req.ip,
    userAgent: req.get("User-Agent"),
    statusCode: err.statusCode || 500,
  });

  // MongoDB/Mongoose errors
  if (err.name === "CastError") error = handleCastErrorDB(error);
  if (err.code === 11000) error = handleDuplicateFieldsDB(error);
  if (err.name === "ValidationError") error = handleValidationErrorDB(error);

  // JWT errors
  if (err.name === "JsonWebTokenError") error = handleJWTError();
  if (err.name === "TokenExpiredError") error = handleJWTExpiredError();

  // Joi validation errors
  if (err.isJoi) error = handleJoiValidationError(error);

  // Redis errors
  if (err.code === "ECONNREFUSED" || err.code === "REDIS_CONNECTION_ERROR") {
    error = handleRedisError(error);
  }

  // Mistral AI errors
  if (err.isAxiosError && err.config?.baseURL?.includes("mistral")) {
    error = handleMistralError(error);
  }

  // Set default values if not set
  error.statusCode = error.statusCode || 500;
  error.status = error.status || "error";
  error.isOperational =
    error.isOperational !== undefined ? error.isOperational : false;

  // Send error response
  if (process.env.NODE_ENV === "development") {
    sendErrorDev(error, res);
  } else {
    sendErrorProd(error, res);
  }
};

// 404 error handler
const notFound = (req, res, next) => {
  const err = new AppError(`Not found - ${req.originalUrl}`, 404);
  next(err);
};

// Async error wrapper
const asyncHandler = (fn) => (req, res, next) => {
  Promise.resolve(fn(req, res, next)).catch(next);
};

module.exports = {
  AppError,
  errorHandler,
  notFound,
  asyncHandler,
};
