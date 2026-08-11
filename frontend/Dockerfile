# syntax=docker/dockerfile:1

# --- Build stage ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# Vite 8 requires Node.js ^20.19 || >=22.12
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl ca-certificates gnupg \
    && curl -fsSL https://deb.nodesource.com/setup_22.x | bash - \
    && apt-get install -y --no-install-recommends nodejs \
    && rm -rf /var/lib/apt/lists/*

# NOTE: elmish-land derives the generated App project name from the
# directory name, so it must match the project name (frontend).
WORKDIR /frontend

# npm dependencies
COPY package.json package-lock.json ./
RUN npm ci

# dotnet tools (elmish-land, fable)
COPY global.json dotnet-tools.json ./
RUN dotnet tool restore

# Project files
COPY Directory.Packages.props frontend.fsproj frontend.slnx elmish-land.json vite.config.js index.html ./
COPY src/ ./src/

# Build the SPA (dotnet elmish-land build -> dist/)
RUN npm run build

# --- Runtime stage ---
FROM nginx:alpine AS runtime

COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /frontend/dist/ /usr/share/nginx/html/

EXPOSE 80
