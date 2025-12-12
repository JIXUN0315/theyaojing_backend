# 1️⃣ 前端：build Vue (Vite)
FROM node:24 AS frontend

WORKDIR /app
COPY ClientApp/ ./ClientApp/
WORKDIR /app/ClientApp
RUN npm install
RUN npm run build

# 2️⃣ 後端：build .NET Web API
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY . .
# 將前端 build 出來的檔案複製進 wwwroot
COPY --from=frontend /app/ClientApp/dist ./wwwroot

RUN dotnet publish -c Release -o /app/publish

# 3️⃣ 運行階段：最小化映像
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Backend.dll"]