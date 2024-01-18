# Use the ASP.NET Core runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["NBL_PoC/NBL_PoC_Api.csproj", "NBL_PoC/"]
RUN dotnet restore "NBL_PoC/NBL_PoC_Api.csproj"

# Copy the entire application source code
COPY . .

# Build the application
WORKDIR "/src/NBL_PoC"
RUN dotnet build "NBL_PoC_Api.csproj" -c Release -o /app/build

# Create a stage for publishing the application
FROM build AS publish
RUN dotnet publish "NBL_PoC_Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Create the final runtime image
FROM base AS final
WORKDIR /app

# Copy the published application from the publish stage
COPY --from=publish /app/publish .

# Download and make the wait-for-it script executable
ADD https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh .
RUN chmod +x wait-for-it.sh

# Set the entry point to wait for the database and then start the application
ENTRYPOINT ["./wait-for-it.sh", "postgres:5432", "--", "dotnet", "NBL_PoC_Api.dll"]
