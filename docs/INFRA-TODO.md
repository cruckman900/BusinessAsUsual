# 🧾 Infrastructure TODO — Three-Instance Split

Follow-up hardening items after the MAIN / HEAVY / LIGHT split went live.
All items are optional polish; the split itself is deployed and verified.

## Topology recap

| Instance | Private IP | Runs | SG |
| --- | --- | --- | --- |
| MAIN | `10.0.1.9` | web, admin, backend, gateway, edge nginx | `bau-prod-sg` |
| HEAVY | `10.0.1.250` | crm-api (5004), hr-api (5041) | `bau-heavy-sg` |
| LIGHT | `10.0.1.175` | moduleregistry (5100), ai-api (5300) | `bau-light-sg` |

---

## TODO

### 1. Lock HEAVY/LIGHT published ports to VPC-only
The service ports (5004/5041/5100/5300) are currently reachable by anything the
SG allows. Confirm they are NOT open to `0.0.0.0/0` and only permit the peer SGs.
- [ ] HEAVY: 5004/5041 inbound source = `bau-prod-sg` only
- [ ] LIGHT: 5100 inbound source = `bau-prod-sg` + `bau-heavy-sg` only
- [ ] LIGHT: 5300 inbound source = `bau-heavy-sg` only (if AI is used)
- [ ] Optionally bind containers to the private IP in compose (e.g.
	  `"10.0.1.250:5004:80"`) so they never listen on the public interface.

### 2. Add a dedicated gateway health route
Give the gateway its own liveness endpoint (separate from proxying `/health`
to the registry) so edge/monitoring can check the gateway itself.
- [ ] Add `location = /gateway-health { return 200 "ok\n"; }` to
	  `deploy/nginx/gateway.conf.template`
- [ ] Re-deploy MAIN and verify `curl -s http://127.0.0.1:8088/gateway-health`

### 3. Commit + push the infra changes
- [ ] `git add` template, compose, workflow, docs
- [ ] Commit "Split prod into three instances with gateway on main"
- [ ] Push to `origin/main`

---

## 🔍 EC2 verification commands (run after each deploy)

SSH/SSM into the relevant instance first. Prompt shows `sh-5.2$` (bash on the host).

### Where does each service actually reside? (which containers, which host)

Run on **each** instance to see what it's running:
```sh
docker ps --format 'table {{.Names}}\t{{.Image}}\t{{.Ports}}\t{{.Status}}'
```

Expected per instance:
- MAIN  → `bau-web`, `bau-admin`, `bau-backend`, `gateway` (nginx on 127.0.0.1:8088)
- HEAVY → `bau-crm-api` (5004), `bau-hr-api` (5041) (+ crm-web/hr-web)
- LIGHT → module registry (5100), ai-api (5300)

### Which compose project / working dir a container came from
```sh
docker inspect <container> \
  --format '{{ index .Config.Labels "com.docker.compose.project"}} | {{ index .Config.Labels "com.docker.compose.project.working_dir"}}'
```

### What ports are actually listening (and on which interface)
```sh
sudo ss -tlnp | grep -E '5004|5041|5100|5300|8088'
```
- `0.0.0.0:PORT` = listening on all interfaces (public-capable)
- `127.0.0.1:PORT` = loopback only (gateway should look like this on MAIN)

### Confirm the rendered gateway upstreams on MAIN
```sh
grep upstream /home/ec2-user/BusinessAsUsual/deploy/nginx/gateway.conf
docker exec businessasusual-gateway-1 grep upstream /etc/nginx/conf.d/default.conf
```
Both should show real IPs (`10.0.1.250:5041`, `10.0.1.250:5004`, `10.0.1.175:5100`),
never literal `${...}` placeholders.

### Private-network reachability (run from MAIN)
```sh
for t in 10.0.1.250:5004 10.0.1.250:5041 10.0.1.175:5100; do
  printf "%s -> " "$t"; curl -s -o /dev/null -w "%{http_code}\n" "http://$t/health"
done
```

### End-to-end through the public gateway
```sh
curl -s https://api.businessasusual.work/api/crm/mobile/ui-spec | grep -oE 'pipeline|email-templates'
```

### Disk health (small instances fill up on rebuild)
```sh
df -h /
docker system df
```
