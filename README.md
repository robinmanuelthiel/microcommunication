![Screenshot of the Web Frontend](assets/Screenshot.png)

# Micro-service Intercommunication Demo

[![Docker](https://img.shields.io/badge/Docker%20Image-API-blue.svg)](https://hub.docker.com/r/robinmanuelthiel/microcommunication-api/) [![Docker](https://img.shields.io/badge/Docker%20Image-Web-blue.svg)](https://hub.docker.com/r/robinmanuelthiel/microcommunication-web/)

This is a small demo project to quickly setup a mix of containerized micro-services that communicate with each other within the network. This demo is intended to provide a playground for orchestrators!

The project consists of

- An API Backend, that exposes port `8080`
- A Web Frontend, that exposes port `80`
  - Connects to the API Backend for getting random numbers using the `RandomApiHost` environment variable

## Make it run

### Prerequisites

- [.NET Core 2 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/products/docker-desktop)

The easiest way to run and debug the microservices on you development machine is [Visual Studio Code](https://code.visualstudio.com/). Just open the folder and select the *Launch Microservices* Debug configuration.

![Screenshot of Visual Studio Code](assets/LaunchInVsCode.png)

## Orchestrators

This Demo project is intended to be tested within container orchestrators. For the various different orchestrators out there, you can find configuration files for multiple different ones in the repository.

> **Windows Containers:** If you want to run on Windows, feel free to use the Windows versions of these containers instead by using the `*.windows.*` files, where available.

### Docker Compose

```bash
docker-compose -f docker-compose.yml up
```

### Kubernetes

```bash
kubectl create -f kubernetes.yml
```

### Service Fabric Mesh

```bash
az mesh deployment create --resource-group Demo --template-file servicefabric-mesh.json
```
