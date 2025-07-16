# Use Node.js 20 LTS
FROM node:20-alpine

# Set working directory
WORKDIR /app

# Install system dependencies for PDF processing
RUN apk add --no-cache \
  python3 \
  make \
  g++ \
  cairo-dev \
  jpeg-dev \
  pango-dev \
  musl-dev \
  giflib-dev \
  pixman-dev \
  pangomm-dev \
  libjpeg-turbo-dev \
  freetype-dev

# Copy package files
COPY package*.json ./

# Install dependencies
RUN npm ci --only=production && npm cache clean --force

# Create non-root user
RUN addgroup -g 1001 -S nodejs && \
  adduser -S nodejs -u 1001

# Create logs directory and set permissions
RUN mkdir -p /app/logs && \
  chown -R nodejs:nodejs /app

# Copy application code
COPY --chown=nodejs:nodejs . .

# Create startup script
RUN echo '#!/bin/sh' > /app/start.sh && \
  echo 'echo "🚀 Starting CV AI Microservice..."' >> /app/start.sh && \
  echo 'echo "🌐 Starting main application server..."' >> /app/start.sh && \
  echo 'node index.js &' >> /app/start.sh && \
  echo 'echo "🤖 Starting AI worker process..."' >> /app/start.sh && \
  echo 'node src/workers/aiWorker.js &' >> /app/start.sh && \
  echo 'wait' >> /app/start.sh && \
  chmod +x /app/start.sh

# Switch to non-root user
USER nodejs

# Expose port
EXPOSE 3000

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD node -e "const http = require('http'); \
  const options = { host: 'localhost', port: 3000, path: '/health', timeout: 2000 }; \
  const req = http.request(options, (res) => { \
  process.exit(res.statusCode === 200 ? 0 : 1); \
  }); \
  req.on('error', () => process.exit(1)); \
  req.end();"

# Start both application and worker
CMD ["/bin/sh", "/app/start.sh"]
