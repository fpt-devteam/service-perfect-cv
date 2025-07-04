const Redis = require("redis");
const logger = require("./logger");

const redisConfig = {
  host: process.env.REDIS_HOST || "localhost",
  port: process.env.REDIS_PORT || 6379,
  password: process.env.REDIS_PASSWORD || undefined,
  retryDelayOnFailover: 100,
  enableReadyCheck: false,
  maxRetriesPerRequest: null,
};

// Create Redis client
const createRedisClient = () => {
  const client = Redis.createClient({
    socket: {
      host: redisConfig.host,
      port: redisConfig.port,
    },
    password: redisConfig.password,
  });

  client.on("connect", () => {
    logger.info("Redis client connected");
  });

  client.on("error", (err) => {
    logger.error("Redis client error:", err);
  });

  client.on("ready", () => {
    logger.info("Redis client ready");
  });

  return client;
};

module.exports = {
  redisConfig,
  createRedisClient,
};
