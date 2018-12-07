# Micro-Services Communication Demo

- Web-Frontend exposes port `80`
- API-Backend exposes port `8080`

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