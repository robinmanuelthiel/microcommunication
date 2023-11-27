![Screenshot of the Web Frontend](assets/Screenshot.png)

# Micro-service Intercommunication Demo

[![Docker](https://img.shields.io/badge/Docker%20Hub-microcommunication--api-blue.svg?logo=docker)](https://hub.docker.com/r/robinmanuelthiel/microcommunication-api/)
&nbsp;
[![Docker](https://img.shields.io/badge/Docker%20Hub-microcommunication--web-blue.svg?logo=docker)](https://hub.docker.com/r/robinmanuelthiel/microcommunication-web/)
&nbsp;
[![Docker](https://img.shields.io/badge/Docker%20Hub-microcommunication--random-blue.svg?logo=docker)](https://hub.docker.com/r/robinmanuelthiel/microcommunication-random/)
&nbsp;
![Build and push Docker images](https://github.com/robinmanuelthiel/microcommunication/workflows/Build%20and%20push%20Docker%20images/badge.svg)
&nbsp;
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=robinmanuelthiel_microcommunication&metric=alert_status)](https://sonarcloud.io/dashboard?id=robinmanuelthiel_microcommunication)

This is a small demo project to quickly setup a mix of containerized micro-services that communicate with each other within the network. This demo is intended to provide a playground for orchestrators!

The project consists of

- An API Gateway, written in .NET Core, exposes port `8080`
- A Radom Number Generator Service, written in .NET Core, exposes port `8080`
- A Web Frontend, written in Angular, exposes port `8080`

## Make it run

### Prerequisites

- [.NET Core 3.1 SDK](https://dotnet.microsoft.com/download)
- [Angular CLI](https://cli.angular.io/)
- [Docker](https://www.docker.com/products/docker-desktop)

The easiest way to run and debug the microservices on you development machine is [Visual Studio Code](https://code.visualstudio.com/). Just open the folder and select one of the Debug configurations.

![Screenshot of Visual Studio Code](assets/LaunchInVsCode.png)

## Environment variables

You need to set some environment variables to configure the services and their discovery.

`MicroCommunication.Api`:

- `ApiKey=test123` _Optional: The key, that the API allows for authorization_
- `RedisCacheConnectionString=...` _Optional: The connection string for a Redis Cache to sync SignalR Hubs_
- `ApplicationInsightsConnectionString=...` _Optional: The Azure Application Insights Instrumentation Key_
- `Cors` _Optional: The domain of your web app to add to the CORS_
- `RandomApiUrl` _The URL to connect to the Random Service_

`MicroCommunication.Random`:

- `MongoDbConnectionString=mongo://...` _Optional: The connection string for a Mongo DB to store the history in_
- `ApplicationInsightsConnectionString=...` _Optional: The Azure Application Insights Instrumentation Key_
- `Cors` _Optional: The domain of your web app to add to the CORS_

`MicroCommunication.Web`:

- `API_URL=http://localhost:8080` _Where to find the API_
- `API_KEY=test123` _Which key to use when calling the API_
- `APPLICATION_INSIGHTS_INSTRUMENTATION_KEY=...` _Optional: The Azure Application Insights Instrumentation Key_

## Deploy

This Demo project is intended to be tested within container orchestrators. For the various different orchestrators out there, you can find configuration files for multiple different ones in the repository.

### Kubernetes

#### Manifests

```bash
kubectl create -f env/kubernetes
```

#### Helm

```bash
helm install microcommunication env/helm \
  --namespace microcommunication \
  --create-namespace
```

#### GitOps with Flux

```bash
helm repo add fluxcd https://charts.fluxcd.io

helm upgrade -i flux fluxcd/flux \
  --set git.url=https://github.com/robinmanuelthiel/microcommunication.git \
  --set git.path=env/kubernetes \
  --set git.readonly=true \
  --namespace flux
```

### Docker Compose

```bash
docker compose -f env/docker/docker-compose.yaml up
```

## Testing

### Performance Test

```bash
k6 tests/performance/script.js
```
