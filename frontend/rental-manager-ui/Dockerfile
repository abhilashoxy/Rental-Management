# ---------- build ----------
FROM node:22-alpine AS build
WORKDIR /app

# copy & install (include dev deps!)
COPY frontend/rental-manager-ui/package*.json ./
# If you have a lockfile, prefer: RUN npm ci --include=dev --legacy-peer-deps
RUN npm install --legacy-peer-deps

# copy sources and build
COPY frontend/rental-manager-ui/. ./
# make SURE the local CLI is present; harmless if already installed via package.json
RUN npx ng version && npm run build

# ---------- runtime ----------
FROM nginx:1.25-alpine
# optional custom nginx; comment this out if you don’t have the file
COPY frontend/rental-manager-ui/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/rental-manager-ui/browser /usr/share/nginx/html
EXPOSE 80
