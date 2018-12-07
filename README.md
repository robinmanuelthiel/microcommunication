# Micro-Services Communication Demo

This is a small demo project to quickly setup a mix of containerized micro-services which communicate with each other within the network. This demo is intended to provide a playground for orchestrators!

Currently, it consists of

- An API-Backend, that exposes port `8080`
- A Web-Frontend, that exposes port `80` and connects to the API-Backend for getting random numbers

## Make it run

### Docker-compose

```bash
docker build -t "microcommunication-api:latest" MicroCommunication.Api/
docker build -t "microcommunication-web:latest" MicroCommunication.Web/

docker-compose -f docker-compose.yml up
```

### Kubernetes

```bash
kubectl create -f kubernetes.yml
```

### Service Fabric Mesh

```bash
z mesh deployment create --resource-group Demo --template-file servicefabric-mesh.json
```