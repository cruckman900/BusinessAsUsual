#!/bin/bash
cd /var/www/BusinessAsUsual
dotnet publish -c Release -o out
sudo systemctl start bau-admin