#######################################################
# Step 1: Build the application in a container        #
#######################################################
FROM node:12-alpine as build
WORKDIR /app

# Resore NPM packages
COPY package.json package-lock.json ./
RUN npm install

# Copy the rest of the files over
COPY . .

# Build the application
RUN npm install -g @angular/cli
RUN ng build --configuration production --output-path=/dist

#######################################################
# Step 2: Run the build outcome in a container        #
#######################################################
FROM nginx:alpine

# Copy the build outputs over to this container
COPY nginx.conf /etc/nginx/nginx.conf

# Copy the NGINX config over
COPY --from=build /dist /usr/share/nginx/html

# Create a new user, who has the rights to modify NGINX files, to allow the container to also run as non-root
RUN adduser -D node root
RUN touch /var/run/nginx.pid && \
  chown -R node:root /run /var/run/nginx.pid /var/cache/nginx /usr/share/nginx/html

# Create startup script to replace placeholders in appsettings.template.js with environment variables and run NGINX
RUN echo "envsubst < /usr/share/nginx/html/assets/appsettings.template.js > /usr/share/nginx/html/assets/appsettings.js" > run.sh && \
  echo "nginx -g 'daemon off;'" >> run.sh
RUN cat run.sh

# Switch to new user
USER node

CMD ["/bin/sh", "run.sh"]
