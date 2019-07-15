![Screenshot of the Web Frontend](assets/Screenshot.png)

# Micro-service Intercommunication Demo

[![Docker](https://img.shields.io/badge/Docker%20Image-robinmanuelthiel%2Fmicrocommunication--api-blue.svg?logo=docker)](https://hub.docker.com/r/robinmanuelthiel/microcommunication-api/) [![Docker](https://img.shields.io/badge/Docker%20Image-robinmanuelthiel%2Fmicrocommunication--web-blue.svg?logo=docker)](https://hub.docker.com/r/robinmanuelthiel/microcommunication-web/)
[![Build Status](https://dev.azure.com/robinmanuelthiel/Microcommunication/_apis/build/status/Build%20and%20push?branchName=master)](https://dev.azure.com/robinmanuelthiel/Microcommunication/_build/latest?definitionId=9&branchName=master)

This is a small demo project to quickly setup a mix of containerized micro-services that communicate with each other within the network. This demo is intended to provide a playground for orchestrators!

The project consists of

- An API Backend, that exposes port `8080`
- A Web Frontend, that exposes port `80`
  - Connects to the API Backend for getting random numbers using the `RandomApiHost` environment variable

## Make it run

### Prerequisites

- [.NET Core 2 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/products/docker-desktop)

The easiest way to run and debug the microservices on you development machine is [Visual Studio Code](https://code.visualstudio.com/). Just open the folder and select the _Launch Microservices_ Debug configuration.

![Screenshot of Visual Studio Code](assets/LaunchInVsCode.png)

## Orchestrators

This Demo project is intended to be tested within container orchestrators. For the various different orchestrators out there, you can find configuration files for multiple different ones in the repository.

> **Windows Containers:** If you want to run on Windows, feel free to use the Windows versions of these containers instead by using the `*.windows.*` files, where available.

### Docker Compose

```bash
docker-compose -f env/docker-compose/docker-compose.yml up
```

### Kubernetes

```bash
kubectl create -f env/kubernetes/kubernetes.yml
```

### Service Fabric Mesh

```bash
az mesh deployment create --resource-group Demo --template-file env/servicefabric-mesh/servicefabric-mesh.json
```
