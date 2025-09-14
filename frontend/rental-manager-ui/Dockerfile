# ---- Build ----
FROM node:22-alpine AS build
WORKDIR /app

# only install deps first (better cache)
COPY frontend/rental-manager-ui/package*.json ./
RUN npm install --legacy-peer-deps

# copy rest of app and build
COPY frontend/rental-manager-ui/. ./
RUN npm run build

# ---- Nginx runtime ----
FROM nginx:1.25-alpine
COPY frontend/rental-manager-ui/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/rental-manager-ui/browser /usr/share/nginx/html
EXPOSE 80
