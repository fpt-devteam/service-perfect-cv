const Queue = require("bull");
const { redisConfig } = require("../config/redis");
const logger = require("../config/logger");

// Create CV Analysis Queue with proper Redis configuration
const cvAnalysisQueue = new Queue("cv analysis", {
  redis: {
    host: redisConfig.host,
    port: redisConfig.port,
    password: redisConfig.password,
    retryDelayOnFailover: redisConfig.retryDelayOnFailover,
    enableReadyCheck: redisConfig.enableReadyCheck,
    maxRetriesPerRequest: redisConfig.maxRetriesPerRequest,
  },
  defaultJobOptions: {
    removeOnComplete: 10, // Keep last 10 completed jobs
    removeOnFail: 50, // Keep last 50 failed jobs
    attempts: 3, // Retry failed jobs 3 times
    backoff: {
      type: "exponential",
      delay: 5000, // Start with 5 second delay
    },
  },
});

// Job types
const JOB_TYPES = {
  ANALYZE_CV: "analyze_cv",
  REANALYZE_CV: "reanalyze_cv",
  GENERATE_SUGGESTIONS: "generate_suggestions",
};

// Add CV analysis job
const addCVAnalysisJob = async (jobData) => {
  try {
    const job = await cvAnalysisQueue.add(JOB_TYPES.ANALYZE_CV, jobData, {
      priority: jobData.priority || 0, // Higher numbers = higher priority
      delay: jobData.delay || 2000, // Minimum 2 second delay to avoid rate limits
    });

    logger.info("CV analysis job added to queue", {
      jobId: job.id,
      cvId: jobData.cvId,
      userId: jobData.userId,
    });

    return job;
  } catch (error) {
    logger.error("Failed to add CV analysis job to queue:", error);
    throw error;
  }
};

// Add reanalysis job
const addReanalysisJob = async (jobData) => {
  try {
    const job = await cvAnalysisQueue.add(JOB_TYPES.REANALYZE_CV, jobData, {
      priority: 5, // Higher priority for reanalysis
    });

    logger.info("CV reanalysis job added to queue", {
      jobId: job.id,
      cvId: jobData.cvId,
      userId: jobData.userId,
    });

    return job;
  } catch (error) {
    logger.error("Failed to add CV reanalysis job to queue:", error);
    throw error;
  }
};

// Add suggestion generation job
const addSuggestionJob = async (jobData) => {
  try {
    const job = await cvAnalysisQueue.add(
      JOB_TYPES.GENERATE_SUGGESTIONS,
      jobData,
      {
        priority: 3, // Medium priority for suggestions
      }
    );

    logger.info("Suggestion generation job added to queue", {
      jobId: job.id,
      cvId: jobData.cvId,
      section: jobData.section,
    });

    return job;
  } catch (error) {
    logger.error("Failed to add suggestion job to queue:", error);
    throw error;
  }
};

// Get job status
const getJobStatus = async (jobId) => {
  try {
    const job = await cvAnalysisQueue.getJob(jobId);

    if (!job) {
      return { status: "not_found" };
    }

    const state = await job.getState();
    const progress = job.progress();

    return {
      id: job.id,
      status: state,
      progress: progress,
      data: job.data,
      createdAt: new Date(job.timestamp),
      processedOn: job.processedOn ? new Date(job.processedOn) : null,
      finishedOn: job.finishedOn ? new Date(job.finishedOn) : null,
      failedReason: job.failedReason,
      attempts: job.attemptsMade,
      maxAttempts: job.opts.attempts,
    };
  } catch (error) {
    logger.error("Failed to get job status:", error);
    throw error;
  }
};

// Get queue statistics
const getQueueStats = async () => {
  try {
    const [waiting, active, completed, failed, delayed] = await Promise.all([
      cvAnalysisQueue.getWaiting(),
      cvAnalysisQueue.getActive(),
      cvAnalysisQueue.getCompleted(),
      cvAnalysisQueue.getFailed(),
      cvAnalysisQueue.getDelayed(),
    ]);

    return {
      waiting: waiting.length,
      active: active.length,
      completed: completed.length,
      failed: failed.length,
      delayed: delayed.length,
      total:
        waiting.length +
        active.length +
        completed.length +
        failed.length +
        delayed.length,
    };
  } catch (error) {
    logger.error("Failed to get queue statistics:", error);
    throw error;
  }
};

// Clean old jobs
const cleanQueue = async () => {
  try {
    // Clean completed jobs older than 24 hours
    await cvAnalysisQueue.clean(24 * 60 * 60 * 1000, "completed");

    // Clean failed jobs older than 7 days
    await cvAnalysisQueue.clean(7 * 24 * 60 * 60 * 1000, "failed");

    logger.info("Queue cleaned successfully");
  } catch (error) {
    logger.error("Failed to clean queue:", error);
  }
};

// Queue event listeners
cvAnalysisQueue.on("completed", (job, result) => {
  logger.info("Job completed", {
    jobId: job.id,
    jobType: job.data.type || "unknown",
    duration: job.finishedOn - job.processedOn,
  });
});

cvAnalysisQueue.on("failed", (job, err) => {
  logger.error("Job failed", {
    jobId: job.id,
    jobType: job.data.type || "unknown",
    error: err.message,
    attempts: job.attemptsMade,
    maxAttempts: job.opts.attempts,
  });
});

cvAnalysisQueue.on("stalled", (job) => {
  logger.warn("Job stalled", {
    jobId: job.id,
    jobType: job.data.type || "unknown",
  });
});

cvAnalysisQueue.on("progress", (job, progress) => {
  logger.debug("Job progress updated", {
    jobId: job.id,
    progress: progress,
  });
});

// Graceful shutdown
process.on("SIGTERM", async () => {
  logger.info("Gracefully closing CV analysis queue...");
  await cvAnalysisQueue.close();
});

// Clean queue every hour
setInterval(cleanQueue, 60 * 60 * 1000);

module.exports = {
  cvAnalysisQueue,
  JOB_TYPES,
  addCVAnalysisJob,
  addReanalysisJob,
  addSuggestionJob,
  getJobStatus,
  getQueueStats,
  cleanQueue,
};
