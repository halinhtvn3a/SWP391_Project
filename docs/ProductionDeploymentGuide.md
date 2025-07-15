# Production Deployment Guide for CourtCaller

## Overview

This document outlines the changes required for production deployment in Docker containers with different appsettings.

## Current Development vs Production Setup

### Development Environment (Current GitHub Configuration)
- All current configurations are optimized for development
- Database seeding is enabled and configured for development data
- Connection strings point to local development databases
- Detailed logging is enabled for debugging

### Production Environment (Docker Container Requirements)

## Configuration Changes Required for Production

### 1. appsettings.Production.json

Create `API/appsettings.Production.json` with the following structure:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Error",
      "Microsoft.EntityFrameworkCore": "Error"
    }
  },
  "ConnectionStrings": {
    "CourtCallerDb": "[PRODUCTION_SQL_SERVER_CONNECTION_STRING]"
  },
  "SeedingConfiguration": {
    "Enabled": true,
    "Environment": "Production",
    "MaxRetryAttempts": 5,
    "RetryDelaySeconds": 5,
    "EnableTransactions": true,
    "BatchSize": 500,
    "EnableBackup": true,
    "EnableValidation": true,
    "ContinueOnError": false
  },
  "JWT": {
    "Secret": "[PRODUCTION_JWT_SECRET_256_BIT]"
  },
  "MailSettings": {
    "Mail": "[PRODUCTION_EMAIL]",
    "DisplayName": "CourtCaller Production",
    "Password": "[PRODUCTION_EMAIL_PASSWORD]",
    "Host": "[PRODUCTION_SMTP_HOST]",
    "Port": 587
  },
  "RedisConfiguration": {
    "Host": "[PRODUCTION_REDIS_HOST]",
    "Port": "[PRODUCTION_REDIS_PORT]",
    "Password": "[PRODUCTION_REDIS_PASSWORD]",
    "Ssl": "true",
    "AbortOnConnectFail": "false"
  }
}
```

### 2. Docker Environment Variables

Set these environment variables in your Docker container:

```bash
# Core Configuration
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080

# Database
ConnectionStrings__CourtCallerDb="[PRODUCTION_SQL_SERVER_CONNECTION_STRING]"

# Security
JWT__Secret="[PRODUCTION_JWT_SECRET_256_BIT]"

# Email Service
MailSettings__Mail="[PRODUCTION_EMAIL]"
MailSettings__Password="[PRODUCTION_EMAIL_PASSWORD]"
MailSettings__Host="[PRODUCTION_SMTP_HOST]"

# Redis Cache
RedisConfiguration__Host="[PRODUCTION_REDIS_HOST]"
RedisConfiguration__Port="[PRODUCTION_REDIS_PORT]"
RedisConfiguration__Password="[PRODUCTION_REDIS_PASSWORD]"
```

### 3. Database Seeding Strategy for Production

The production seeding will execute with the following priorities:

1. **Core Data Seeding (Priority 1)**: Essential system data
2. **Reference Data Seeding (Priority 2)**: Lookup tables and configurations
3. **Test Data Seeding (Priority 3)**: Disabled in production by default

#### Production Seeding Files Structure
```
CourtCaller.Persistence/Data/Seed/
├── Production/
│   ├── Branches.json          # Production branch data
│   ├── Courts.json            # Production court data
│   ├── Prices.json            # Production pricing data
│   ├── TimeSlots.json         # Production time slots
│   └── Users.json             # Production admin users
└── Development/               # Keep existing dev data
```

### 4. Security Considerations for Production

#### Changes Required:
1. **CORS Policy**: Update to production domains only
   ```csharp
   // In Program.cs, update CORS policy
   policy.WithOrigins("https://yourdomain.com", "https://api.yourdomain.com")
   ```

2. **JWT Secret**: Use 256-bit randomly generated secret
3. **HTTPS Enforcement**: Ensure HTTPS redirection is enabled
4. **Database Connection**: Use encrypted connection strings

### 5. Performance Optimizations for Production

#### Logging Changes:
- Reduce log levels to Warning/Error only
- Disable detailed EF Core query logging
- Enable structured logging for monitoring

#### Database Configuration:
- Connection pooling optimization
- Reduced seeding batch sizes (500 vs 1000)
- Enhanced retry mechanisms

#### Memory and Resources:
- Optimize dependency injection lifetimes
- Configure appropriate garbage collection
- Set memory limits in Docker container

### 6. Monitoring and Health Checks

#### Required Production Monitoring:
1. **Application Health**: `/health` endpoint
2. **Database Health**: Connection and seeding status
3. **External Services**: Redis, Email service connectivity
4. **Performance Metrics**: Response times, memory usage

### 7. Docker Configuration

#### Sample Dockerfile additions:
```dockerfile
# Set production environment
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1
```

### 8. Data Migration Strategy

#### Production Deployment Process:
1. **Backup existing database** (if applicable)
2. **Run EF Core migrations**: `dotnet ef database update`
3. **Execute seeding**: Automatic on startup if enabled
4. **Verify seeding results**: Check logs and database state
5. **Rollback plan**: Database restore if issues occur

### 9. Security Environment Variables (Critical)

#### Must be set in production:
```bash
# Generate these securely for production
JWT__Secret="[64-character-random-string]"
ConnectionStrings__CourtCallerDb="[encrypted-connection-string]"
MailSettings__Password="[app-specific-password]"
RedisConfiguration__Password="[redis-auth-token]"
```

### 10. Testing Production Configuration

#### Before deployment, verify:
- [ ] Database connectivity
- [ ] Redis connectivity  
- [ ] Email service functionality
- [ ] JWT token generation/validation
- [ ] Seeding execution with production data
- [ ] CORS policy with production domains
- [ ] HTTPS certificate configuration
- [ ] Health check endpoints

## Seeding Configuration Options (Reference)

Below are the key seeding configuration options and recommended values for production deployments. These settings should be placed in your `appsettings.Production.json` or set as environment variables in your Docker container.

### Global Seeding Settings

| Setting              | Description                        | Recommended (Production) | Notes                       |
|----------------------|------------------------------------|--------------------------|-----------------------------|
| `Enabled`            | Enable/disable seeding             | `true`                   | Set to `false` to skip seed |
| `EnableBackup`       | Create backup before seeding        | `true`                   | Strongly recommended        |
| `EnableRollback`     | Rollback on failure                | `true`                   |                             |
| `MaxRetryAttempts`   | Max retry attempts                 | `5`                      | For transient errors        |
| `TimeoutSeconds`     | Operation timeout (seconds)        | `300`                    |                             |
| `ForceReseed`        | Reseed existing data               | `false`                  | Use with caution           |
| `ValidateAfterSeed`  | Validate after completion          | `true`                   |                             |

### Environment-Specific Seeding

| Setting             | Development | Staging | Production |
|---------------------|-------------|---------|------------|
| `SeedCoreData`      | ✅          | ✅      | ✅         |
| `SeedReferenceData` | ✅          | ✅      | ✅         |
| `SeedTestData`      | ✅          | ✅      | ❌         |
| `SeedDemoData`      | ✅          | ❌      | ❌         |
| `BatchSize`         | 2000        | 500     | 100        |

**Production Recommendations:**
- Only enable `SeedCoreData` and `SeedReferenceData` in production.
- Disable `SeedTestData` and `SeedDemoData` to avoid test/demo data in live systems.
- Use a smaller `BatchSize` (e.g., 100) for safer, more controlled seeding.
- Always enable backup and rollback for safety.

**Example (appsettings.Production.json):**
```json
{
  "SeedingConfiguration": {
    "Enabled": true,
    "EnableBackup": true,
    "EnableRollback": true,
    "MaxRetryAttempts": 5,
    "TimeoutSeconds": 300,
    "ForceReseed": false,
    "ValidateAfterSeed": true,
    "Production": {
      "SeedCoreData": true,
      "SeedReferenceData": true,
      "SeedTestData": false,
      "SeedDemoData": false,
      "BatchSize": 100,
      "ExcludedEntities": []
    }
  }
}
```

## Critical Notes for Deployment Team

⚠️ **IMPORTANT**: The following values MUST be changed for production:

1. **JWT Secret**: Generate a secure 256-bit secret
2. **Connection Strings**: Use production database credentials
3. **Email Credentials**: Use production email service
4. **CORS Origins**: Restrict to production domains only
5. **Redis Configuration**: Use production Redis instance
6. **Logging Levels**: Reduce to Warning/Error for performance

## Support and Troubleshooting

### Common Production Issues:
1. **Seeding Failures**: Check database permissions and connection
2. **JWT Issues**: Verify secret configuration and length
3. **CORS Errors**: Ensure production domains are whitelisted
4. **Performance**: Monitor memory usage and connection pooling

### Log Monitoring:
- Monitor startup seeding logs
- Track authentication failures
- Watch for database connection issues
- Monitor external service failures

---

**Contact**: Development Team for configuration support
**Last Updated**: January 2025
**Environment**: Docker Production Deployment 