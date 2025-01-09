## User Management Microservice

# CQRS Microservice for User Management - makes use of JWT authentication

# ToDo:
1) Implement Refit
1) Add role based authorization

# Docker

Build image => docker build --no-cache -f Dockerfile -t user-management-service .
Run Container => docker run --rm -p <ConfiguredPortNumber>:8000 user-management-service

