FROM mcr.microsoft.com/dotnet/sdk:8.0.404 AS base
WORKDIR /app
EXPOSE 5179

FROM mcr.microsoft.com/dotnet/sdk:8.0.404 AS build
WORKDIR /src
COPY ["UserManagementService.Api.Data/UserManagementService.Api.Data.csproj", "Api/Data"]
COPY ["UserManagementService.Api.Domain/UserManagementService.Api.Domain.csproj", "Api/Domain"]
COPY ["UserManagementService.Api.WebApplication/UserManagementService.Api.WebApplication.csproj", "Api/WebApplication"]
COPY . .
WORKDIR "/src"
RUN dotnet restore "UserManagementService.Api.WebApplication/UserManagementService.Api.WebApplication.csproj"
RUN dotnet build "UserManagementService.Api.WebApplication/UserManagementService.Api.WebApplication.csproj" -c Release -o /app/build -v diag

FROM build AS publish
RUN dotnet publish "UserManagementService.Api.WebApplication/UserManagementService.Api.WebApplication.csproj" -c Release -o /app/publish /p:UseAppHost=false -v diag

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "UserManagementService.Api.WebApplication.dll" ]
