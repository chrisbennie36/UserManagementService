FROM mcr.microsoft.com/dotnet/sdk:8.0.404 AS base
WORKDIR /app
EXPOSE 5175

# Ensure we listen on any IP Address 
ENV DOTNET_URLS=http://+:8000

FROM mcr.microsoft.com/dotnet/sdk:8.0.404 AS build
WORKDIR /src
COPY ["Api/UserManagementService.Api.Data/UserManagementService.Api.Data.csproj", "Api/Data"]
COPY ["Api/UserManagementService.Api.Domain/UserManagementService.Api.Domain.csproj", "Api/Domain"]
COPY ["Api/UserManagementService.Api.WebApplication/UserManagementService.Api.WebApplication.csproj", "Api/WebApplication"]
COPY ["Shared/UserManagementService.Shared.Enums/UserManagementService.Shared.Enums.csproj", "Shared/Enums"]
COPY . .
WORKDIR "/src"
RUN dotnet restore "Api/UserManagementService.Api.WebApplication/UserManagementService.Api.WebApplication.csproj"
RUN dotnet build "Api/UserManagementService.Api.WebApplication/UserManagementService.Api.WebApplication.csproj" -c Release -o /app/build -v diag

FROM build AS publish
RUN dotnet publish "Api/UserManagementService.Api.WebApplication/UserManagementService.Api.WebApplication.csproj" -c Release -o /app/publish /p:UseAppHost=false -v diag

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "UserManagementService.Api.WebApplication.dll" ]
