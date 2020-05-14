# Code Analysis with Sonar Cloud

You can use this project, to showcase static code analysis with [Sonar Cloud](https://sonarcloud.io/). The analysis can be done during the Docker image build process. All you need to do is passing some build arguments to the build process.

```bash
docker build . \
  --build-arg SONAR_ANALYZE=true \
  --build-arg SONAR_TOKEN=xxxxx
```
