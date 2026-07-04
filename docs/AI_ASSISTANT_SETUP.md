# AI Assistant Setup

The in-app AI assistant is served by the **`AI.Api`** microservice (`services/AI/AI.Api`).
It exposes `POST /api/ai/chat` and routes each request to one of two tiers:

| Tier | Provider | Model | When it's used |
| ---- | -------- | ----- | -------------- |
| **Demo** (free) | GitHub Models | `gpt-4o-mini` | Default for everyone, including the public `admin`/`password` test case |
| **Paid** | Amazon Bedrock | `anthropic.claude-3-haiku-20240307-v1:0` | Only when the request carries a `companyId` that exists in the provisioning master DB |

Provider choice is **config-driven** (`Ai` section in `appsettings.json`), so other models
(OpenAI, Gemini, Azure OpenAI, Ollama) can be dropped in later without code changes.

---

## 1. Demo tier — GitHub Models (free)

GitHub Models is free (rate-limited) and needs only a GitHub token with the **`models`** scope.

Create a token: GitHub → Settings → Developer settings → **Fine-grained tokens** (or a classic
token) → enable **Models** access.

### Dev (User Secrets — never committed)

```powershell
cd "services\AI\AI.Api"
dotnet user-secrets set "Ai:Demo:ApiKey" "ghp_your_token_here"
```

### Deploy (environment variable)

```
Ai__Demo__ApiKey=ghp_your_token_here
```

If no token is set, the demo tier is simply disabled (the service logs a warning and still starts).

---

## 2. Paid tier — Claude Haiku via Amazon Bedrock

Requires an AWS account with **Bedrock model access enabled** for Claude 3 Haiku
(AWS Console → Bedrock → Model access → request access).

Credentials use the **standard AWS SDK credential chain** — do **not** put AWS keys in config.
Provide them by any one of:

- Environment variables:
  ```
  AWS_ACCESS_KEY_ID=...
  AWS_SECRET_ACCESS_KEY=...
  AWS_REGION=us-east-1
  ```
- A shared profile (`~/.aws/credentials`), or
- An IAM role (recommended in production / on EC2 / ECS).

Region and model id are set in `appsettings.json` under `Ai:Paid`.

### Company gate (master DB)

The paid tier only unlocks when `companyId` matches a row in the `Companies` table of the
provisioning master DB. `AI.Api` reads the same connection string the backend uses:

```
AWS_SQL_CONNECTION_STRING=Server=...;Database=BusinessAsUsual;User Id=...;Password=...;
```

If this variable is unset, company validation is skipped and **every** request stays on the
demo tier (fails closed — paid access is never granted without verification).

---

## 3. Running it

Start `AI.Api` alongside the web app. In Visual Studio, set **multiple startup projects**
(`BusinessAsUsual.Web` + `AI.Api`), or from the CLI:

```powershell
dotnet run --project "services\AI\AI.Api\AI.Api.csproj"
```

It listens on `http://localhost:5300` (see `Properties/launchSettings.json`). The web app's
`AiApi` HttpClient points here via the `AiService:Url` config key.

---

## 4. Abuse & cost guards

- **Rate limiting** — 10 requests/min per client IP (HTTP 429 when exceeded).
- **Input cap** — prompts over `Ai:MaxInputChars` (default 1000) are rejected before any model call.
- **Output cap** — `Ai:MaxOutputTokens` (default 512) limits generation length.
- **Paid gate** — Bedrock (metered) is only reachable with a valid provisioned `companyId`.
- The demo tier's free ceiling means the worst public-abuse case is "temporarily rate-limited,"
  not a surprise bill.

---

## 5. Adding another provider later

1. Add its package to `AI.Api.csproj`.
2. Create an `IChatClient` (most SDKs offer `.AsIChatClient()`; otherwise write a small adapter
   like `BedrockChatClient`).
3. Construct it in `Program.cs` based on an `Ai` config value and pass it into `AiClientRegistry`.

No controller or widget changes are needed — everything talks to `IChatClient`.
