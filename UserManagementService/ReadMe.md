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

# Database

Makes use of a PostgreSQL database and uses Migrations for a code first approach

# Architecture

Microservice is deployed in AWS as an ElasticBeanstalk application. The database is deployed in AWS as a Postgres 16.3 Engine with a security group which cotains an ingress rule allowing this Microservice to communicate with it

# Docker

Build image => docker build -f Dockerfile -t user-management-service .
Run Container => docker run --rm -p <ConfiguredPortNumber>:8000 user-management-service

