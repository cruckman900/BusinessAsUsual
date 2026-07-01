# HR Service Environment Variables

# Add this to your .env.local file in the project root:

# HR Service Database Connection
HR_SQL_CONNECTION_STRING=Server=localhost;Database=BusinessAsUsual_HR;Trusted_Connection=True;TrustServerCertificate=True;

# Notes:
# - The HR service will use ConfigLoader to read from .env.local in Development
# - In Production, it will attempt to load from AWS Secrets Manager
# - The database name is BusinessAsUsual_HR (separate from main DB)
# - This follows the database-per-service pattern for microservices
