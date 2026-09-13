FROM node:22-alpine AS build
WORKDIR /app
COPY frontend/package*.json ./
RUN npm ci
COPY frontend/ .
RUN npm run build

FROM nginx:1.29-alpine
ARG API_UPSTREAM=api:8080
COPY infra/docker/nginx/frontend.conf /etc/nginx/templates/default.conf.template
COPY --from=build /app/dist/frontend/browser /usr/share/nginx/html
RUN sed -i "s/__API_UPSTREAM__/${API_UPSTREAM}/g" /etc/nginx/templates/default.conf.template
EXPOSE 80
