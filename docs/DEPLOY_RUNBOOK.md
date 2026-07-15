# Deploy Runbook — BusinessAsUsual API behind `api.businessasusual.work`

This runbook deploys the **contract-driven microservices stack**
(Module Registry + HR + CRM + nginx gateway) to the EC2 host that serves
`api.businessasusual.work`, so the Android app can discover modules over one
public HTTPS URL.

The same Docker images run locally and in production — only `PUBLIC_BASE_URL`
and TLS differ. This is not a throwaway/dev shortcut; it is the deployable
stack.

---

## 0. What you're deploying

| Piece | File | Role |
|-------|------|------|
| Base stack | `docker-compose.microservices.yml` | Registry + HR + CRM + gateway on one Docker network |
| Prod overrides | `docker-compose.microservices.prod.yml` | Public HTTPS URL, restart policy, gateway bound to `127.0.0.1:8088` |
| In-cluster router | `deploy/nginx/gateway.conf` | Path-routes `/api/hr/`, `/api/crm/`, `/api/modules/` |
| Edge TLS/proxy | `deploy/nginx/edge-api.businessasusual.work.conf` | Host nginx: terminates TLS, forwards to gateway |

Request path once deployed:

```
Phone → https://api.businessasusual.work (edge nginx, TLS)
	  → http://127.0.0.1:8088 (compose gateway)
	  → hr-api / crm-api / moduleregistry (Docker network)
```

---

## 1. Find and access the EC2 box

You said you're unsure how to get in. Work through this in order.

### 1.1 Locate the instance (AWS Console)
1. Sign in to the **AWS Console** → search **EC2** → **Instances**.
2. Find the running instance. Confirm it's the right one by matching its
   **public IPv4 address** to what the domain resolves to:
   ```powershell
   nslookup api.businessasusual.work
   ```
   The IP returned should match an instance's public IP (or an Elastic IP
   under **EC2 → Elastic IPs**).
3. Note the instance's **Public IPv4 DNS**, **Key pair name**, and
   **Security group**.

### 1.2 Choose an access method

**Option A — SSM Session Manager (no SSH key needed; preferred if enabled):**
1. EC2 → select instance → **Connect** → **Session Manager** → **Connect**.
2. If greyed out, the instance needs the `AmazonSSMManagedInstanceCore` IAM
   role + the SSM agent (Amazon Linux 2023 has it preinstalled). This is the
   easiest recovery path when the SSH key is lost.

**Option B — SSH with the key pair:**
1. You need the `.pem` private key matching the instance's key pair name.
2. Ensure the **security group** allows inbound **TCP 22** from your IP.
   ```
   ssh -i path\to\key.pem ec2-user@<PUBLIC_DNS>     # Amazon Linux
   ssh -i path\to\key.pem ubuntu@<PUBLIC_DNS>       # Ubuntu
   ```

**Lost the key and SSM is off?** Recover via: stop instance → detach root
volume → attach to a helper instance → add a new public key to
`~/.ssh/authorized_keys` → reattach → boot. (Or re-create from an AMI.) Ask me
and I'll walk you through it.

### 1.3 Confirm the security group opens the web ports
Inbound rules must allow:
- **80** (HTTP, for certbot challenge + redirect)
- **443** (HTTPS, the phone's traffic)
- **22** (SSH, your IP only — optional if using SSM)

Do **not** expose 8088; it stays loopback-only.

---

## 2. Install prerequisites on the box (one time)

```bash
# Docker + compose plugin (Amazon Linux 2023)
sudo dnf install -y docker git
sudo systemctl enable --now docker
sudo usermod -aG docker $USER   # log out/in after this
# compose plugin
sudo dnf install -y docker-compose-plugin || {
  DOCKER_CONFIG=${DOCKER_CONFIG:-/usr/local/lib/docker}
  sudo mkdir -p $DOCKER_CONFIG/cli-plugins
  sudo curl -SL https://github.com/docker/compose/releases/latest/download/docker-compose-linux-x86_64 \
	   -o $DOCKER_CONFIG/cli-plugins/docker-compose
  sudo chmod +x $DOCKER_CONFIG/cli-plugins/docker-compose
}

# nginx + certbot for the edge
sudo dnf install -y nginx
sudo systemctl enable --now nginx
sudo dnf install -y certbot python3-certbot-nginx
```

---

## 3. Get the code and run the stack

```bash
git clone https://github.com/cruckman900/BusinessAsUsual.git
cd BusinessAsUsual

export PUBLIC_BASE_URL=https://api.businessasusual.work

docker compose \
  -f docker-compose.microservices.yml \
  -f docker-compose.microservices.prod.yml \
  up --build -d

# Verify inside the box (gateway is loopback-only):
curl -s localhost:8088/health
curl -s localhost:8088/api/modules/mobile | head -c 300
```

You should see modules returned by `/api/modules/mobile`.

---

## 4. Wire the edge nginx + TLS

```bash
# Install the edge server block
sudo cp deploy/nginx/edge-api.businessasusual.work.conf \
		/etc/nginx/conf.d/api.businessasusual.work.conf
sudo nginx -t && sudo systemctl reload nginx

# Issue the certificate (adds the 443 block + HTTP→HTTPS redirect)
sudo certbot --nginx -d api.businessasusual.work
sudo systemctl reload nginx
```

> If the box already has an existing config answering `api.businessasusual.work`
> (that's the 404 nginx we saw), remove/replace that server block first so ours
> takes effect. Check `/etc/nginx/conf.d/` and `/etc/nginx/sites-enabled/`.

---

## 5. Verify from the outside

From your PC:
```powershell
curl.exe -s https://api.businessasusual.work/health
curl.exe -s https://api.businessasusual.work/api/modules/mobile
curl.exe -s https://api.businessasusual.work/api/hr/mobile/ui-spec
```
All should return `200` with JSON (not the old 404).

---

## 6. Point the Android app at the domain (done)

In the Android repo, `data/build.gradle.kts` release build's AWS URLs now use
the public domain:

```kotlin
buildConfigField("String", "AWS_HR_URL",       "\"https://api.businessasusual.work/\"")
buildConfigField("String", "AWS_REGISTRY_URL",  "\"https://api.businessasusual.work/\"")
```

In a **release** build `AWS_FIRST` puts the domain first, and
`FailoverInterceptor` rewrites the scheme/host/port to HTTPS:443 automatically.

### 6.1 Release signing (required to install)

The `release` build type previously produced `app-release-unsigned.apk`, which
a device will refuse to install. Signing is now wired up:

- Keystore: `app/bau-release.jks` (alias `bau`)
- Credentials: `keystore.properties` (git-ignored)
- `app/build.gradle.kts` loads `keystore.properties` into a
  `signingConfigs.release` and applies it to the `release` build type.

> ⚠️ **Back up `app/bau-release.jks` and `keystore.properties`.** They are
> git-ignored, so if lost you cannot ship signed updates to the same app
> identity. AppsOnAir requires every update to keep the same signature.

### 6.2 Build the signed APK

```powershell
cd "D:\Android Projects\BusinessAsUsual_Android"
.\gradlew.bat :app:assembleRelease
```

Output (signed, installable):

```
D:\Android Projects\BusinessAsUsual_Android\app\build\outputs\apk\release\app-release.apk
```

### 6.3 Distribute via AppsOnAir

1. Upload `app-release.apk` to AppsOnAir.
2. AppsOnAir gives you a public install/download URL and a QR code.
3. Update the website to point at that URL (see below), then install from the
   AppsOnAir link / QR and confirm modules load and the nav/hamburger work.

Website wiring for the AppsOnAir link lives in:
- `frontend/BusinessAsUsual.Web/Configuration/AppLinks.cs` — set
  `AndroidInstallUrl` to the AppsOnAir install URL.
- `frontend/BusinessAsUsual.Web/Pages/AppDownload.razor` — renders the install
  button + QR image from that URL.

---

## 7. Operations notes

- **Restart:** `docker compose -f docker-compose.microservices.yml -f docker-compose.microservices.prod.yml restart`
- **Logs:** `docker compose -f docker-compose.microservices.yml logs -f hr-api`
- **Registry data is in-memory** (`UseInMemoryDatabase=true`): services
  re-register on startup + heartbeat, so a restart is self-healing. *Phase 2:*
  switch to a persistent DB (RDS/SQL) for durability across full stack restarts.
- **Adding a module later:** build its image, add a service + gateway
  `location /api/<mod>/` block, set its `*__ApiBaseUrl` to the public origin,
  redeploy. See `docs/MODULE_BLUEPRINT.md`.

## 8. Two independent stacks share this box — don't cross the streams

The EC2 host runs **two separate Docker Compose stacks**. They are unrelated
and must be started/stopped independently.

| Stack | Compose file(s) | Serves |
|-------|-----------------|--------|
| Monolith | `docker-compose.prod.yml` | `businessasusual.work` (web :3000), `admin.businessasusual.work` (admin :8080), backend :5000 |
| Microservices | `docker-compose.microservices.yml` + `docker-compose.microservices.prod.yml` | `api.businessasusual.work` (gateway :8088) |

- **NEVER use `--remove-orphans`** when tearing down one stack. Because both
  stacks live on the same host, `--remove-orphans` deletes the *other* stack's
  containers (this is exactly what caused a 502 on `businessasusual.work` /
  `admin.businessasusual.work` — the monolith containers on :3000/:8080 were
  removed while restarting the microservices API).
- Safe microservices restart (leaves the monolith alone):
  ```bash
  docker compose -f docker-compose.microservices.yml -f docker-compose.microservices.prod.yml down
  docker compose -f docker-compose.microservices.yml -f docker-compose.microservices.prod.yml up --build -d
  ```
- If the monolith ever gets stopped, bring it back with:
  ```bash
  docker compose -f docker-compose.prod.yml up -d
  ```
- Both stacks now set `restart: unless-stopped`, so containers auto-recover on
  reboot/crash — but that does **not** protect against `--remove-orphans`, which
  deletes the container outright.
- Quick health probe for all upstreams:
  ```bash
  curl -s -o /dev/null -w "web(3000): %{http_code}\n"   localhost:3000/
  curl -s -o /dev/null -w "admin(8080): %{http_code}\n" localhost:8080/
  curl -s -o /dev/null -w "api(8088): %{http_code}\n"   localhost:8088/health
  ```

