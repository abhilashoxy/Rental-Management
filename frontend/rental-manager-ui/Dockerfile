# ---- Build ----
FROM node:22-alpine AS build
WORKDIR /app

# Install dependencies
COPY frontend/rental-manager-ui/package*.json ./
RUN npm install --legacy-peer-deps

# Copy rest of the app and build
COPY frontend/rental-manager-ui/. ./
RUN npm run build

# ---- Nginx runtime ----
FROM nginx:1.25-alpine
COPY frontend/rental-manager-ui/nginx.conf /etc/nginx/conf.d/default.conf
# Adjust the path below if your build output is different
COPY --from=build /app/dist/rental-manager-ui/browser /usr/share/nginx/html
EXPOSE 80
