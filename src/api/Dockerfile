#######################################################
# Step 1: Build the application in a container        #
#######################################################
# Download the official ASP.NET Core SDK image
# to build the project while creating the docker image
FROM pumpingcode/dotnet-core-advanced-build:latest as build
WORKDIR /app

ARG SONAR_ANALYZE=false
ARG SONAR_PROJECT_KEY=robinmanuelthiel_microcommunication
ARG SONAR_ORGANIZATION_KEY=robinmanuelthiel
ARG SONAR_HOST_URL=https://sonarcloud.io
ARG SONAR_BRANCH=master
ARG SONAR_TOKEN

# Start the Sonar Scanner
RUN /sonar/sonar-start.sh

# Restore NuGet packages
COPY ["MicroCommunication.Api/MicroCommunication.Api.csproj", "MicroCommunication.Api/"]
COPY ["MicroCommunication.Api.Tests/MicroCommunication.Api.Tests.csproj", "MicroCommunication.Api.Tests/"]
RUN dotnet restore MicroCommunication.Api/MicroCommunication.Api.csproj
RUN dotnet restore MicroCommunication.Api.Tests/MicroCommunication.Api.Tests.csproj

# Copy the rest of the files over
COPY . .

# Build the application
RUN dotnet publish --output /out/ --configuration Debug MicroCommunication.Api/MicroCommunication.Api.csproj

# Test the application
RUN dotnet tool install --global coverlet.console --version 1.7.1
RUN dotnet test --configuration Debug MicroCommunication.Api.Tests/MicroCommunication.Api.Tests.csproj \
  /p:CollectCoverage=true \
  /p:CoverletOutputFormat=opencover \
  /p:CoverletOutput="/coverage"

# End the Sonar Scanner
RUN /sonar/sonar-end.sh

#######################################################
# Step 2: Run the build outcome in a container        #
#######################################################
# Download the official ASP.NET Core Runtime image
# to run the compiled application
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app

# Open port
EXPOSE 8080

# Copy the build output from the SDK image
COPY --from=build /out .

# Start the application
ENTRYPOINT ["dotnet", "MicroCommunication.Api.dll"]
