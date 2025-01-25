# DockerFile for local development environment

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/FrameUp.OrderService.Api/FrameUp.OrderService.Api.csproj", "src/FrameUp.OrderService.Api/"]
COPY ["src/FrameUp.OrderService.Application/FrameUp.OrderService.Application.csproj", "src/FrameUp.OrderService.Application/"]
COPY ["src/FrameUp.OrderService.Domain/FrameUp.OrderService.Domain.csproj", "src/FrameUp.OrderService.Domain/"]
COPY ["src/FrameUp.OrderService.Infra/FrameUp.OrderService.Infra.csproj", "src/FrameUp.OrderService.Infra/"]
RUN dotnet restore "./src/FrameUp.OrderService.Api/FrameUp.OrderService.Api.csproj"
COPY . .
WORKDIR "src/FrameUp.OrderService.Api"
RUN dotnet build "./FrameUp.OrderService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FrameUp.OrderService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FrameUp.OrderService.Api.dll"]