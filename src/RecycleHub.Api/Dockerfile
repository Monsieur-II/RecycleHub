# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ReceycleHub.sln ./
COPY src/RecycleHub.Api/RecycleHub.Api.csproj src/RecycleHub.Api/
COPY src/RecycleHub.Pg.Sdk/RecycleHub.Pg.Sdk.csproj src/RecycleHub.Pg.Sdk/
COPY src/RecycleHub.Utils/RecycleHub.Utils.csproj src/RecycleHub.Utils/

# Restore dependencies
RUN dotnet restore "src/RecycleHub.Api/RecycleHub.Api.csproj"

# Copy the rest of the source
COPY . .

# Build and publish
WORKDIR /src/src/RecycleHub.Api
RUN dotnet publish "RecycleHub.Api.csproj" -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create non-root user
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "RecycleHub.Api.dll"]
