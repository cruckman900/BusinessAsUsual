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
