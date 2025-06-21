# Use the official .NET 9.0 runtime image as the base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official .NET 9.0 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["ElectionsAppApi/ElectionsAppApi/ElectionsAppApi.csproj", "ElectionsAppApi/ElectionsAppApi/"]
RUN dotnet restore "ElectionsAppApi/ElectionsAppApi/ElectionsAppApi.csproj"

# Copy the rest of the source code
COPY . .
WORKDIR "/src/ElectionsAppApi/ElectionsAppApi"

# Build the application
RUN dotnet build "ElectionsAppApi.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "ElectionsAppApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Build the final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production

# Create a non-root user for security
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Start the application
ENTRYPOINT ["dotnet", "ElectionsAppApi.dll"] 