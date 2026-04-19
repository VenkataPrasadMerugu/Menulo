# =========================
# BUILD STAGE
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["Menulo.Web/Menulo.Web.csproj", "Menulo.Web/"]
COPY ["Menulo.Application/Menulo.Application.csproj", "Menulo.Application/"]
COPY ["Menulo.Infrastructure/Menulo.Infrastructure.csproj", "Menulo.Infrastructure/"]
COPY ["Menulo.Domain/Menulo.Domain.csproj", "Menulo.Domain/"]

# Restore dependencies
RUN dotnet restore "Menulo.Web/Menulo.Web.csproj"

# Copy all files
COPY . .

# Build and publish
WORKDIR "/src/Menulo.Web"
RUN dotnet publish "Menulo.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# =========================
# RUNTIME STAGE
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# Required for Render
ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 10000

ENTRYPOINT ["dotnet", "Menulo.Web.dll"]