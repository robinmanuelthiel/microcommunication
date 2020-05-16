#######################################################
# Step 1: Build the application in a container        #
#######################################################
# Download the official ASP.NET Core SDK image
# to build the project while creating the docker image
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build
ARG SONAR_ANALYZE=false
ARG SONAR_PROJECT_KEY=robinmanuelthiel_microcommunication
ARG SONAR_OGRANIZAION_KEY=robinmanuelthiel
ARG SONAR_HOST_URL=https://sonarcloud.io
ARG SONAR_TOKEN
WORKDIR /app

# Install Sonar Scanner and Java (required for Sonar Scanner)
RUN if [ "$SONAR_ANALYZE" = "true" ] ; then apt-get update && apt-get install -y openjdk-11-jdk ; fi
RUN if [ "$SONAR_ANALYZE" = "true" ] ; then dotnet tool install --global dotnet-sonarscanner ; fi
ENV PATH="$PATH:/root/.dotnet/tools"

# Start Sonar Scanner
RUN if [ "$SONAR_ANALYZE" = "true" ] ; then\
  dotnet sonarscanner begin \
  /k:"$SONAR_PROJECT_KEY" \
  /o:"$SONAR_OGRANIZAION_KEY" \
  /d:sonar.host.url="$SONAR_HOST_URL" \
  /d:sonar.login="$SONAR_TOKEN" \
  /d:sonar.cs.opencover.reportsPaths=/coverage.opencover.xml \
  ; fi

# Restore NuGet packages
COPY ["src/MicroCommunication.Api/MicroCommunication.Api.csproj", "src/MicroCommunication.Api/"]
COPY ["tests/MicroCommunication.Api.Tests/MicroCommunication.Api.Tests.csproj", "tests/MicroCommunication.Api.Tests/"]
RUN dotnet restore src/MicroCommunication.Api/MicroCommunication.Api.csproj
RUN dotnet restore tests/MicroCommunication.Api.Tests/MicroCommunication.Api.Tests.csproj

# Copy the rest of the files over
COPY . .

# Build the application
RUN dotnet publish --output /out/ --configuration Debug src/MicroCommunication.Api/MicroCommunication.Api.csproj

# Test the application
RUN dotnet tool install --global coverlet.console
RUN dotnet test --configuration Debug tests/MicroCommunication.Api.Tests/MicroCommunication.Api.Tests.csproj \
  /p:CollectCoverage=true \
  /p:CoverletOutputFormat=opencover \
  /p:CoverletOutput="/coverage"

# End Sonar Scanner
RUN if [ "$SONAR_ANALYZE" = "true" ] ; then dotnet sonarscanner end /d:sonar.login="$SONAR_TOKEN" ; fi

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
