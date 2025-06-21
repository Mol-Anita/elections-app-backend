# Elections App Backend

A .NET 9.0 Web API for managing election candidates with real-time WebSocket support for candidate generation.

## Features

- RESTful API for CRUD operations on candidates
- Real-time WebSocket communication for candidate generation
- In-memory data storage (can be extended to use a database)
- Health check endpoints for monitoring
- CORS support for frontend integration

## API Endpoints

### Candidates
- `GET /api/candidates` - Get all candidates
- `GET /api/candidates/{id}` - Get candidate by ID
- `POST /api/candidates` - Create new candidate
- `PUT /api/candidates/{id}` - Update candidate
- `DELETE /api/candidates/{id}` - Delete candidate

### Health Checks
- `GET /health` - Health check endpoint
- `GET /health/ready` - Readiness check
- `GET /health/live` - Liveness check

### WebSocket
- `WS /ws/candidates` - WebSocket endpoint for real-time candidate generation

## WebSocket Commands

Send JSON messages to the WebSocket endpoint:

```json
{"type": "start"}  // Start candidate generation
{"type": "stop"}   // Stop candidate generation
```

## Environment Variables

- `ASPNETCORE_ENVIRONMENT` - Environment (Development/Production)
- `ASPNETCORE_URLS` - URLs to bind to (default: http://0.0.0.0:$PORT)
- `CORS_ALLOWED_ORIGINS` - CORS allowed origins (default: "*")
- `WEBSOCKET_ENABLED` - Enable WebSocket support (default: "true")

## Deployment

### Railway Deployment

1. **Connect your repository to Railway**
   - Go to [Railway](https://railway.app)
   - Create a new project
   - Connect your GitHub repository

2. **Configure environment variables** (optional)
   - Set `ASPNETCORE_ENVIRONMENT=Production`
   - Configure any other environment variables as needed

3. **Deploy**
   - Railway will automatically detect the .NET project
   - The `railway.toml` file will configure the deployment
   - The application will be available at your Railway URL

### Docker Deployment

1. **Build the Docker image**
   ```bash
   docker build -t elections-app-backend .
   ```

2. **Run the container**
   ```bash
   docker run -p 8080:80 elections-app-backend
   ```

3. **Access the application**
   - API: http://localhost:8080/api/candidates
   - Health check: http://localhost:8080/health
   - WebSocket: ws://localhost:8080/ws/candidates

### Local Development

1. **Prerequisites**
   - .NET 9.0 SDK
   - Visual Studio 2022 or VS Code

2. **Run the application**
   ```bash
   cd ElectionsAppApi
   dotnet run
   ```

3. **Access the application**
   - API: https://localhost:7186/api/candidates
   - Health check: https://localhost:7186/health
   - WebSocket: wss://localhost:7186/ws/candidates

## Configuration Files

- `railway.toml` - Railway deployment configuration
- `Dockerfile` - Docker container configuration
- `.dockerignore` - Files to exclude from Docker build

## Health Monitoring

The application includes health check endpoints that can be used by monitoring systems:

- `/health` - Basic health check
- `/health/ready` - Readiness probe (for Kubernetes)
- `/health/live` - Liveness probe (for Kubernetes)

## CORS Configuration

The application is configured to allow CORS from any origin by default. In production, you should configure specific origins:

```json
{
  "CORS_ALLOWED_ORIGINS": "https://your-frontend-domain.com"
}
```

## WebSocket Usage

The WebSocket endpoint supports real-time candidate generation:

1. **Connect to WebSocket**
   ```javascript
   const ws = new WebSocket('ws://localhost:8080/ws/candidates');
   ```

2. **Send commands**
   ```javascript
   ws.send(JSON.stringify({type: 'start'}));  // Start generation
   ws.send(JSON.stringify({type: 'stop'}));   // Stop generation
   ```

3. **Receive messages**
   ```javascript
   ws.onmessage = (event) => {
     const message = JSON.parse(event.data);
     if (message.type === 'candidate') {
       console.log('New candidate:', message.data);
     }
   };
   ```

## Troubleshooting

### Common Issues

1. **WebSocket connection fails**
   - Ensure the WebSocket endpoint is enabled
   - Check if the frontend is using the correct WebSocket URL
   - Verify CORS settings

2. **Health check fails**
   - Check if the application is running
   - Verify the health endpoint is accessible
   - Check application logs for errors

3. **CORS errors**
   - Configure `CORS_ALLOWED_ORIGINS` environment variable
   - Ensure the frontend domain is included in allowed origins

### Logs

The application logs important events including:
- WebSocket connections and disconnections
- Candidate generation events
- API requests and responses
- Health check requests

Check the logs for detailed information about application behavior and errors. 