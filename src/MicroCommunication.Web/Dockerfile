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

# Start NGINX as webserver
CMD ["/bin/sh",  "-c",  "envsubst < /usr/share/nginx/html/assets/appsettings.template.js > /usr/share/nginx/html/assets/appsettings.js && exec nginx -g 'daemon off;'"]
