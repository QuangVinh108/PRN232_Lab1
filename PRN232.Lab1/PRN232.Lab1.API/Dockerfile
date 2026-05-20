# 1. Môi trường chạy (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# 2. Môi trường Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy các file .csproj để restore packages trước
COPY ["PRN232.Lab1.API/PRN232.Lab1.API.csproj", "PRN232.Lab1.API/"]
COPY ["PRN232.Lab1.Services/PRN232.Lab1.Services.csproj", "PRN232.Lab1.Services/"]
COPY ["PRN232.Lab1.Repositories/PRN232.Lab1.Repositories.csproj", "PRN232.Lab1.Repositories/"]
RUN dotnet restore "PRN232.Lab1.API/PRN232.Lab1.API.csproj"

# Copy toàn bộ source code còn lại vào
COPY . .
WORKDIR "/src/PRN232.Lab1.API"
RUN dotnet build "PRN232.Lab1.API.csproj" -c Release -o /app/build

# 3. Publish ứng dụng
FROM build AS publish
RUN dotnet publish "PRN232.Lab1.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. Copy kết quả publish vào môi trường chạy
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PRN232.Lab1.API.dll"]