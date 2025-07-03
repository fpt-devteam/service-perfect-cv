require("dotenv").config();
const express = require("express");
const cors = require("cors");
const helmet = require("helmet");
const connectDB = require("./src/config/database");
const logger = require("./src/config/logger");
const { createRedisClient } = require("./src/config/redis");

const cvRoutes = require("./src/routes/cvRoutes");
const qaRoutes = require("./src/routes/qaRoutes");
const feedbackRoutes = require("./src/routes/feedbackRoutes");

const { globalRateLimiter } = require("./src/middleware/rateLimiter");
const { errorHandler } = require("./src/middleware/errorHandler");
const { requestLogger } = require("./src/middleware/requestLogger");

const app = express();
const PORT = process.env.PORT || 3000;

connectDB();

let redisClient;
(async () => {
  try {
    redisClient = createRedisClient();
    await redisClient.connect();
    logger.info("Redis client connected successfully");
  } catch (error) {
    logger.error("Failed to connect to Redis:", error);
    process.exit(1);
  }
})();

app.use(
  helmet({
    contentSecurityPolicy: {
      directives: {
        defaultSrc: ["'self'"],
        styleSrc: ["'self'", "'unsafe-inline'"],
        scriptSrc: ["'self'"],
        imgSrc: ["'self'", "data:", "https:"],
      },
    },
    crossOriginResourcePolicy: { policy: "cross-origin" },
  })
);

app.use(
  cors({
    origin: process.env.ALLOWED_ORIGINS?.split(",") || [
      "http://localhost:3000",
      "http://localhost:3001",
    ],
    credentials: true,
    methods: ["GET", "POST", "PUT", "DELETE", "OPTIONS"],
    allowedHeaders: ["Content-Type", "Authorization", "X-Requested-With"],
  })
);

app.use(
  express.json({
    limit: process.env.MAX_REQUEST_SIZE || "10mb",
    strict: true,
  })
);
app.use(
  express.urlencoded({
    extended: true,
    limit: process.env.MAX_REQUEST_SIZE || "10mb",
  })
);

app.use(requestLogger);

app.use(globalRateLimiter);

app.get("/health", async (req, res) => {
  try {
    const dbStatus =
      require("mongoose").connection.readyState === 1
        ? "connected"
        : "disconnected";

    let redisStatus = "disconnected";
    try {
      await redisClient.ping();
      redisStatus = "connected";
    } catch (error) {
      redisStatus = "disconnected";
    }

    const healthCheck = {
      status: "OK",
      timestamp: new Date().toISOString(),
      uptime: process.uptime(),
      environment: process.env.NODE_ENV || "development",
      version: process.env.npm_package_version || "1.0.0",
      services: {
        database: dbStatus,
        redis: redisStatus,
        mistral: process.env.MISTRAL_API_KEY ? "configured" : "not_configured",
      },
      memory: {
        used: Math.round(process.memoryUsage().heapUsed / 1024 / 1024) + " MB",
        total:
          Math.round(process.memoryUsage().heapTotal / 1024 / 1024) + " MB",
      },
    };

    const httpStatus =
      dbStatus === "connected" && redisStatus === "connected" ? 200 : 503;
    res.status(httpStatus).json(healthCheck);
  } catch (error) {
    logger.error("Health check failed:", error);
    res.status(503).json({
      status: "ERROR",
      timestamp: new Date().toISOString(),
      error: "Health check failed",
    });
  }
});

app.use("/api/cv", cvRoutes);
app.use("/api/qa", qaRoutes);
app.use("/api/feedback", feedbackRoutes);

app.get("/api", (req, res) => {
  res.json({
    name: "CV AI Microservice",
    version: process.env.npm_package_version || "1.0.0",
    description: "AI-powered CV analysis microservice using Mistral AI",
    endpoints: {
      cv: {
        "POST /api/cv/submit": "Submit CV for AI analysis",
        "GET /api/cv/:cvId/status": "Get CV processing status",
        "GET /api/cv/:cvId/feedback": "Get AI feedback for CV",
        "POST /api/cv/:cvId/apply-suggestion": "Apply improvement suggestion",
        "POST /api/cv/:cvId/reanalyze": "Reanalyze CV with updated content",
        "GET /api/cv/:cvId/history": "Get CV version history",
        "GET /api/cv/user/:userId": "Get user CVs",
      },
      qa: {
        "POST /api/qa/ask": "Ask question to AI assistant",
        "GET /api/qa/session/:sessionId": "Get conversation history",
        "POST /api/qa/session/:sessionId/close": "Close conversation session",
        "GET /api/qa/user/:userId/sessions": "Get user sessions",
        "GET /api/qa/user/:userId/analytics": "Get conversation analytics",
      },
      feedback: {
        "GET /api/feedback/user/:userId/history": "Get feedback history",
        "GET /api/feedback/user/:userId/dashboard": "Get progress dashboard",
        "POST /api/feedback/user/:userId/apply-suggestion":
          "Apply suggestion to history",
        "GET /api/feedback/user/:userId/analytics": "Get improvement analytics",
        "GET /api/feedback/user/:userId/compare": "Compare CV versions",
      },
    },
  });
});

app.get("/", (req, res) => {
  res.json({
    message: "CV AI Microservice is running!",
    status: "OK",
    timestamp: new Date().toISOString(),
    endpoints: {
      health: "/health",
      api: "/api",
      cv: "/api/cv",
      qa: "/api/qa",
      feedback: "/api/feedback",
    },
  });
});

app.use("*", (req, res) => {
  res.status(404).json({
    success: false,
    message: "API endpoint not found",
    path: req.originalUrl,
    method: req.method,
    availableEndpoints: "/api",
  });
});

app.use(errorHandler);

const gracefulShutdown = async (signal) => {
  logger.info(`Received ${signal}, starting graceful shutdown...`);

  server.close(async () => {
    logger.info("HTTP server closed");

    try {
      await require("mongoose").connection.close();
      logger.info("MongoDB connection closed");

      if (redisClient) {
        await redisClient.quit();
        logger.info("Redis connection closed");
      }

      logger.info("Graceful shutdown completed");
      process.exit(0);
    } catch (error) {
      logger.error("Error during graceful shutdown:", error);
      process.exit(1);
    }
  });

  setTimeout(() => {
    logger.error("Force shutdown due to timeout");
    process.exit(1);
  }, parseInt(process.env.SHUTDOWN_TIMEOUT || "30000"));
};

const server = app.listen(PORT, () => {
  logger.info(`CV AI Microservice started on port ${PORT}`, {
    environment: process.env.NODE_ENV || "development",
    port: PORT,
    nodeVersion: process.version,
    timestamp: new Date().toISOString(),
  });
});

server.on("error", (error) => {
  if (error.code === "EADDRINUSE") {
    logger.error(`Port ${PORT} is already in use`);
  } else {
    logger.error("Server error:", error);
  }
  process.exit(1);
});

process.on("SIGTERM", () => gracefulShutdown("SIGTERM"));
process.on("SIGINT", () => gracefulShutdown("SIGINT"));

process.on("uncaughtException", (error) => {
  logger.error("Uncaught Exception:", error);
  process.exit(1);
});

process.on("unhandledRejection", (reason, promise) => {
  logger.error("Unhandled Rejection at:", promise, "reason:", reason);
  process.exit(1);
});

module.exports = app;
