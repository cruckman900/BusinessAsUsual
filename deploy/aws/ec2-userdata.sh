#!/bin/bash
# EC2 user-data: bootstrap Docker + compose plugin on Amazon Linux 2023 so the
# GitHub Actions deploy workflows can run `docker compose ... up -d --build`.
set -euxo pipefail

dnf update -y
dnf install -y docker rsync

systemctl enable --now docker
usermod -aG docker ec2-user

# Install the docker compose v2 plugin (provides `docker compose`)
mkdir -p /usr/local/lib/docker/cli-plugins
curl -SL https://github.com/docker/compose/releases/latest/download/docker-compose-linux-x86_64 \
  -o /usr/local/lib/docker/cli-plugins/docker-compose
chmod +x /usr/local/lib/docker/cli-plugins/docker-compose

# Install a modern docker buildx plugin. The buildx bundled with the AL2023
# docker package is too old for `docker compose build` (needs >= 0.17.0).
curl -SL https://github.com/docker/buildx/releases/download/v0.17.1/buildx-v0.17.1.linux-amd64 \
  -o /usr/local/lib/docker/cli-plugins/docker-buildx
chmod +x /usr/local/lib/docker/cli-plugins/docker-buildx

# Prepare the deploy target directory used by the workflows (rsync target)
mkdir -p /home/ec2-user/BusinessAsUsual
chown -R ec2-user:ec2-user /home/ec2-user/BusinessAsUsual
