# Business As Usual – AWS Deployment Log

```bash
   ,     #_
   ~\_  ####_        Amazon Linux 2023
  ~~  \_#####\
  ~~     \###|
  ~~       \#/ ___   https://aws.amazon.com/linux/amazon-linux-2023
   ~~       V~' '->
    ~~~         /
      ~~._.   _/
         _/ _/
       _/m/'
```

## 1. Goal

Deploy the **Business As Usual** SaaS application to **Amazon Web Services (AWS)** using a cost-conscious, résumé-ready architecture:

- Single **EC2 t3.micro** instance
- Docker + docker-compose
- Domain: `https://businessasusual.work`
- DNS via **Route 53**
- HTTPS via **AWS Certificate Manager (ACM)**
- No NAT Gateway, no load balancer, no surprise billing

This document captures the exact steps taken to stand up the environment.

---

## 2. Account and region

- **AWS Account ID:** 283784618079
- **Plan:** Upgraded from free tier to paid
- **Region:** `us-east-1` (N. Virginia) (chosen for low latency and broad service support)

---

## 3. EC2 instance provisioning

### 3.1. Instance details

- **Service:** Amazon EC2
- **Instance type:** `t3.micro`
- **AMI:** (chosen at deploy time)
  - Option A: Amazon Linux 2023
  - Option B: Ubuntu Server 22.04 LTS
- **Storage:** 16–30 GB gp3
- **Network:** Default VPC
- **Subnet:** Public subnet
- **Security group:**
  - Allow SSH from my IP (`22/tcp`)
  - Allow HTTP (`80/tcp`)
  - Allow HTTPS (`443/tcp`)

### 3.2. SSH access

- Generated or selected an existing key pair.
- Connected via:

```bash
ssh -i /path/to/key.pem ec2-user@<EC2_PUBLIC_IP>   # Amazon Linux
# or
ssh -i /path/to/key.pem ubuntu@<EC2_PUBLIC_IP>     # Ubuntu
```

## 4. Install Docker and Docker Compose (Ubuntu 22.04)

With the EC2 instance provisioned and accessible via SSH, the next step is to install Docker and Docker Compose. These tools allow the entire Business As Usual stack to run in isolated containers, ensuring consistent behavior across development, staging, and production environments.

### 4.1. Update system packages

```bash
sudo apt update && sudo apt upgrade -y
```

# Install Docker Engine

```bash
sudo apt install -y docker.io
sudo systemctl enable docker
sudo systemctl start docker
sudo usermod -aG docker $USER
```

# Verify Docker Installation

```bash
docker --version
docker ps
```

# Install Docker Compose

```bash
sudo curl -L "https://github.com/docker/compose/releases/download/v2.29.0/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose
docker-compose version
```

# Confirm Installation

```bash
docker --version
docker-compose version
```

## 5. Deploying Business As Usual on EC2

With Docker and Docker Compose installed, the next step is to deploy the Business As Usual application onto the EC2 instance.

### 5.1. Clone the repository

From the EC2 terminal:

```bash
git clone https://github.com/cruckman900/BusinessAsUsual.git
cd BusinessAsUsual
```

### 5.2. Environment configuration

```bash
cp .env.example .env

docker-compose up -d --build
docker ps
```

# Test the application locally on EC2

```bash
curl http://localhost
```

## 6. Assign Elastic IP and configure DNS (Route 53)

To ensure a stable public endpoint for the application, an Elastic IP is attached to the EC2 instance and mapped to the custom domain via Route 53.

### 6.1. Allocate and associate an Elastic IP

1. Open the **EC2** console.
2. Navigate to **Network & Security → Elastic IPs**.
3. Click **Allocate Elastic IP address** and confirm.
4. Select the newly allocated Elastic IP and choose **Actions → Associate Elastic IP address**.
5. Associate it with:
   - **Resource type:** Instance
   - **Instance:** The Business As Usual EC2 instance

Record the Elastic IP address for DNS configuration.

### 6.2. Configure Route 53 DNS

1. Open the **Route 53** console.
2. Go to **Hosted zones** and select the hosted zone for `businessasusual.work`.
3. Create or update the following records:

- **Root domain**

  - Type: `A`
  - Name: `businessasusual.work`
  - Value: `<Elastic_IP>`

- **Optional subdomains**
  - `api.businessasusual.work` → `<Elastic_IP>`
  - `admin.businessasusual.work` → `<Elastic_IP>`

### 6.3. Verify DNS resolution

From a local machine:

```bash
ping businessasusual.work
```

You should see the Elastic IP address in the response.
Once DNS propagates, visiting http://businessasusual.work should reach the EC2 instance.

## 7. Enable HTTPS for Business As Usual

To provide secure access, HTTPS is enabled for the domain. For Phase 1, TLS termination is handled directly on the EC2 instance using Nginx and Let’s Encrypt.
Note: A future phase may move TLS termination to an AWS Application Load Balancer with ACM certificates.

### 7.1. Install Nginx

On the EC2 instance:

```bash
sudo apt update
sudo apt install -y nginx
sudo systemctl enable nginx
sudo systemctl start nginx
```

### 7.2. Configure Nginx as a reverse proxy

Create or edit a site configuration, for example:

```bash
sudo nano /etc/nginx/sites-available/businessasusual
```

Example configuration:

```Nginx
server {
    listen 80;
    server_name businessasusual.work www.businessasusual.work;

    location / {
        proxy_pass http://localhost:80; # or the internal port your app exposes
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

Enable the site and reload Nginx:

```bash
sudo ln -s /etc/nginx/sites-available/businessasusual /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx
```

### 7.3. Install Certbot (Let’s Encrypt)

```bash
sudo apt install -y certbot python3-certbot-nginx
```

### 7.4. Obtain and configure HTTPS certificate

```bash
sudo certbot --nginx -d businessasusual.work -d www.businessasusual.work
```

Follow the prompts to:

- Agree to terms
- Choose HTTP → HTTPS redirection
  Certbot will automatically update the Nginx configuration to use HTTPS.

### 7.5. Verify HTTPS

Visit:

- https://businessasusual.work
  Confirm:
- The site loads
- The browser shows a valid HTTPS certificate

## 8. RDS-ready configuration (future migration)

Although Phase 1 uses a Dockerized database on EC2 to minimize cost, the application is prepared for a future migration to Amazon RDS with minimal changes:

### 8.1. Environment variable strategy

The application reads database configuration from environment variables, for example:

- DB_HOST
- DB_PORT
- DB_USER
- DB_PASSWORD
- DB_NAME

### 8.2. Local Docker database configuration

For Phase 1, .env points to the Docker database container, e.g.:

```Env
DB_HOST=db
DB_PORT=5432
DB_USER=bau_user
DB_PASSWORD=bau_password
DB_NAME=business_as_usual
```

### 8.3. RDS configuration template

Create a separate file, e.g. .env.rds, with placeholders for RDS:

```env
DB_HOST=<rds-endpoint.amazonaws.com>
DB_PORT=5432
DB_USER=<rds_username>
DB_PASSWORD=<rds_password>
DB_NAME=<rds_db_name>
```

When ready to migrate to RDS:

- Create the RDS instance (e.g., PostgreSQL db.t3.micro).
- Update .env.rds with the actual RDS endpoint and credentials.
- Replace .env with .env.rds (or load it via deployment tooling).
- Restart the Docker stack:

```bash
docker-compose down
docker-compose up -d --build
```

This design allows a “flip the switch” migration from local Docker DB to RDS with no code changes.

## 9. Cost-conscious design notes

The deployment is intentionally designed to minimize AWS costs while remaining production-relevant and résumé-ready.

- Single EC2 instance (t3.micro):
- Hosts API, frontend, database (Docker), and Nginx.
- Can be stopped when not in use to reduce compute charges.
- No NAT Gateway:
- Avoids high hourly and data processing costs.
- All necessary traffic flows through the public subnet and security groups.
- No load balancer in Phase 1:
- HTTPS is terminated directly on EC2 via Nginx and Let’s Encrypt.
- An Application Load Balancer can be introduced later if needed.
- No RDS in Phase 1:
- Database runs in a Docker container to avoid idle RDS charges.
- RDS migration is planned and documented for future production hardening.
- Route 53 DNS:
- Low, predictable monthly cost for hosted zone and records.

## 10. Future enhancements and Phase 2 plan

Planned future improvements include:

- Database migration to Amazon RDS:
- Create RDS instance (PostgreSQL or MySQL).
- Migrate schema and data from Docker DB.
- Update environment configuration to point to RDS.
- Tighten security groups to allow DB access only from the EC2 instance.
- Application Load Balancer (ALB) and ACM:
- Move TLS termination to an ALB.
- Use AWS Certificate Manager for managed certificates.
- Enable path-based routing or multiple services behind the ALB.
- Auto-scaling and additional instances:
- Introduce an Auto Scaling Group for the application layer.
- Scale horizontally based on CPU, memory, or request metrics.
- Centralized logging and monitoring:
- Use CloudWatch Logs and metrics.
- Set log retention policies to control storage costs.

## 11. Résumé and portfolio summary

Deployed Business As Usual to AWS using:

- Amazon EC2 (t3.micro) running Ubuntu 22.04 LTS
- Docker + Docker Compose for application and database containers
- Nginx as a reverse proxy and TLS termination point
- Custom domain via Route 53: https://businessasusual.work
- Let’s Encrypt for HTTPS certificates
- Cost-conscious architecture:
- No NAT Gateway
- No load balancer in Phase 1
- No idle RDS charges
- RDS-ready configuration:
- Environment-driven DB configuration
- Documented migration path from Docker DB → Amazon RDS
  This deployment demonstrates practical experience with:
- Cloud infrastructure on AWS
- Linux server administration
- Containerized application deployment
- DNS and TLS configuration
- Cost-optimized, production-relevant architecture design
