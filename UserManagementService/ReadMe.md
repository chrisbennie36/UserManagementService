## User Management Microservice

# .Net Core based Microservice which handles the creation and management of Users. Makes use of a PostgreSQL database and uses Keycloak to handle authorization

# Libraries used

    - MediatR
    - FluentValidation
    - EntityFramework
    - XUnit
    - AutoMapper
    - Serilog
    - NSwag

# Docker

Build image => docker build -f Dockerfile -t user-management-service .
Run Container => docker run --rm -p <ConfiguredPortNumber>:8000 user-management-service

