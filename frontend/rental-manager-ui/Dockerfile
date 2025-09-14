# Build stage
FROM node:22-alpine AS build
WORKDIR /app
COPY rental-manager-ui/package*.json ./
RUN npm install --legacy-peer-deps
COPY rental-manager-ui/. ./
RUN npm run build

# Serve with nginx
FROM nginx:1.25-alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/rental-manager-ui/browser /usr/share/nginx/html
EXPOSE 80
