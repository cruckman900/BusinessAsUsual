🚀 ONBOARDING.md — From Clone to Cloud
This file walks new collaborators through the setup process with clarity, speed, and zero guesswork. It’s your backstage pass to getting Business As Usual up and running.

🧭 Overview
- Goal: Get devs from zero to running locally, then Dockerized, then cloud-deployed.
- Tone: Empowering, expressive, and timestamp-friendly. Every step should feel like a win.

🛠️ Local Setup
- Clone the repo
- Install dependencies (Node.js, .NET SDK, etc.)
- Run frontend (npm run dev) and backend (dotnet run)
- Confirm health endpoints and splash screen trigger

### AI Developer Quickstart (local)

1. Start the main solution services (APIs and frontend) using Visual Studio or `dotnet run` for each project.
2. Start the AI microservice (services/AI/AI.Api). From the solution root:

```powershell
cd "services/AI/AI.Api"
dotnet run
```

3. (Optional) Start a local vector DB for development. Recommended options:
   - Qdrant: run `docker run -p 6333:6333 qdrant/qdrant` and use port 6333
   - FAISS (local): use the provided utility scripts to create an on-disk index

4. Seed embeddings using the helper endpoint (future): POST to `/api/ai/embeddings/upsert` with sample CRM data. For now, the AI API accepts placeholder payloads.

5. Run the Blazor frontend and verify the AI assistant icon appears in the top bar. Use the assistant to send a test query (placeholder response currently).

6. Telemetry: enable Application Insights or Prometheus options described in docs/ARCHITECTURE.md to capture AI latency and usage.

Troubleshooting:
- If the assistant isn't visible, ensure `frontend/BusinessAsUsual.Web/wwwroot/js/ai-assistant.js` and `css/ai-assistant.css` are included in the published assets and that `AiAssistant` component is compiled.
- For vector DB connection issues, check firewall/port and ensure the container is running.

🐳 Docker Setup
- Build and run containers for frontend and backend
- Use .env files for local secrets
- Confirm container health and port mapping
- Log milestone in CHANGELOG.md

☁️ AWS Deployment
- Set up AWS account (free tier notes included)
- Configure EC2 or Elastic Beanstalk
- Push Docker images to ECR
- Wire up domain, SSL, and metadata

📓 Bonus Tips
- Use CHANGELOG.md to log every win
- Customize splash screen and Now Playing ticker
- Check LICENSE.md before sharing or demoing
