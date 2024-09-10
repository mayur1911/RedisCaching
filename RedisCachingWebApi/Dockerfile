# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["RedisCachingWebApi.csproj", "./"]
RUN dotnet restore "RedisCachingWebApi.csproj"

# Copy the entire project and build it
COPY . .
RUN dotnet build "RedisCachingWebApi.csproj" -c Release -o /app/build

# Stage 2: Publish the application
FROM build AS publish
RUN dotnet publish "RedisCachingWebApi.csproj" -c Release -o /app/publish

# Stage 3: Create the runtime image
FROM mcr.microsoft.com/mssql/server:2019-latest AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Install sqlcmd tools
RUN apt-get update && apt-get install -y mssql-tools unixodbc-dev

# Add sqlcmd to the path
RUN echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> ~/.bashrc

# Copy the published files from the publish stage
COPY --from=publish /app/publish .

# Start the application
ENTRYPOINT ["dotnet", "RedisCachingWebApi.dll"]
