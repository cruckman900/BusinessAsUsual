#!/bin/bash

echo "🧠 Starting database initialization..."
echo "📄 Running init.sql against db..."

/opt/mssql-tools/bin/sqlcmd -S db -U sa -P 'YourStrong!Password123' -i /init.sql

exit_code=$?

if [ $exit_code -eq 0 ]; then
  echo "✅ init.sql executed successfully."
else
  echo "❌ init.sql failed with exit code $exit_code."
  exit $exit_code
fi