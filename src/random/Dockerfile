#######################################################
# Step 1: Build the application in a container        #
#######################################################
# Download the official ASP.NET Core SDK image
# to build the project while creating the docker image
FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
WORKDIR /app

# Restore NuGet packages
COPY ["MicroCommunication.Random/MicroCommunication.Random.csproj", "MicroCommunication.Random/"]
COPY ["MicroCommunication.Random.Tests/MicroCommunication.Random.Tests.csproj", "MicroCommunication.Random.Tests/"]
RUN dotnet restore MicroCommunication.Random/MicroCommunication.Random.csproj
RUN dotnet restore MicroCommunication.Random.Tests/MicroCommunication.Random.Tests.csproj

# Copy the rest of the files over
COPY . .

# Build the application
RUN dotnet publish --output /out/ --configuration Release MicroCommunication.Random/MicroCommunication.Random.csproj

# Test the application
RUN dotnet test --configuration Release MicroCommunication.Random.Tests/MicroCommunication.Random.Tests.csproj

#######################################################
# Step 2: Run the build outcome in a container        #
#######################################################
# Download the official ASP.NET Core Runtime image
# to run the compiled application
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

# Open port
EXPOSE 8080

# Copy the build output from the SDK image
COPY --from=build /out .

# Start the application
ENTRYPOINT ["dotnet", "MicroCommunication.Random.dll"]
