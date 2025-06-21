#!/bin/bash

echo "=== Elections App API Startup Script ==="
echo "Environment: $ASPNETCORE_ENVIRONMENT"
echo "Port: $PORT"
echo "URLs: $ASPNETCORE_URLS"
echo "Working Directory: $(pwd)"
echo "Files in current directory:"
ls -la

echo "=== Starting .NET Application ==="
dotnet ElectionsAppApi.dll 