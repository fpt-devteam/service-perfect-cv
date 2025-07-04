// Local development startup script
// This loads the .env.local file and starts both the application and worker
const dotenv = require("dotenv");
const path = require("path");
const { spawn } = require("child_process");

// Load local environment variables
const envResult = dotenv.config({
  path: path.resolve(__dirname, ".env.local"),
});

if (envResult.error) {
  console.error("Error loading .env.local file:", envResult.error);
  console.log("Make sure .env.local exists and contains your configuration");
  process.exit(1);
}

console.log("🚀 Starting CV AI Microservice in LOCAL DEVELOPMENT mode");
console.log("📋 Configuration:");
console.log("   • MongoDB:", process.env.MONGODB_URI);
console.log(
  "   • Redis:",
  `${process.env.REDIS_HOST}:${process.env.REDIS_PORT}`
);
console.log("   • Port:", process.env.PORT);
console.log("   • Environment:", process.env.NODE_ENV);
console.log(
  "   • Mistral API Key:",
  process.env.MISTRAL_API_KEY
    ? `${process.env.MISTRAL_API_KEY.substring(0, 8)}...`
    : "❌ NOT SET"
);
console.log("   • Mistral Model:", process.env.MISTRAL_MODEL);
console.log("");

// Start the main application
console.log("🌐 Starting main application server...");
const mainApp = spawn("node", ["index.js"], {
  stdio: "inherit",
  env: process.env,
});

// Start the AI worker
console.log("🤖 Starting AI worker process...");
const aiWorker = spawn("node", ["src/workers/aiWorker.js"], {
  stdio: "inherit",
  env: process.env,
});

// Handle process termination
const cleanup = () => {
  console.log("\n🛑 Shutting down processes...");
  mainApp.kill();
  aiWorker.kill();
  process.exit(0);
};

process.on("SIGINT", cleanup);
process.on("SIGTERM", cleanup);

// Handle process exits
mainApp.on("exit", (code) => {
  console.log(`Main application exited with code ${code}`);
  aiWorker.kill();
  process.exit(code);
});

aiWorker.on("exit", (code) => {
  console.log(`AI worker exited with code ${code}`);
  mainApp.kill();
  process.exit(code);
});

console.log("✅ Both processes started successfully!");
console.log(
  "   • Main API server: http://localhost:" + (process.env.PORT || 3000)
);
console.log("   • AI Worker: Processing CV analysis jobs");
console.log("\nPress Ctrl+C to stop both processes");
