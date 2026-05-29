FROM node:22-alpine AS deps
WORKDIR /app
COPY apps/admin/package*.json ./
RUN npm ci

FROM node:22-alpine AS builder
WORKDIR /app
COPY apps/admin/ .
COPY --from=deps /app/node_modules ./node_modules
ARG NEXT_PUBLIC_API_URL
ENV NEXT_PUBLIC_API_URL=$NEXT_PUBLIC_API_URL
RUN npm run build

FROM node:22-alpine AS runner
WORKDIR /app
ENV NODE_ENV=production
COPY --from=deps /app/node_modules ./node_modules
COPY --from=builder /app/.next ./.next
COPY --from=builder /app/public ./public
COPY --from=builder /app/package*.json ./
EXPOSE 3000
CMD ["npm", "start"]
