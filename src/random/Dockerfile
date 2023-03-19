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
COPY ["MicroCommunication.Random/MicroCommunication.Random.csproj", "MicroCommunication.Random/"]
COPY ["MicroCommunication.Random.Tests/MicroCommunication.Random.Tests.csproj", "MicroCommunication.Random.Tests/"]
RUN dotnet restore MicroCommunication.Random/MicroCommunication.Random.csproj
RUN dotnet restore MicroCommunication.Random.Tests/MicroCommunication.Random.Tests.csproj

# Copy the rest of the files over
COPY . .

# Build the application
RUN dotnet publish --output /out/ --configuration Debug MicroCommunication.Random/MicroCommunication.Random.csproj

# Test the application
RUN dotnet tool install --global coverlet.console --version 1.7.1
RUN dotnet test --configuration Debug MicroCommunication.Random.Tests/MicroCommunication.Random.Tests.csproj \
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
ENTRYPOINT ["dotnet", "MicroCommunication.Random.dll"]
