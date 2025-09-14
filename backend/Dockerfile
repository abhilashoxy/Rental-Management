# =============================
# Build stage
# =============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# (optional) keep NuGet fallback clean
COPY NuGet.config ./

# 1) copy only csproj for layer-cached restore
COPY RentalManagementService/*.csproj ./RentalManagementService/
RUN dotnet restore ./RentalManagementService/RentalManagementService.csproj

# 2) now copy the rest of the source
COPY RentalManagementService/ ./RentalManagementService/

# 3) publish (no restore again)
RUN dotnet publish ./RentalManagementService/RentalManagementService.csproj -c Release -o /app/publish

# =============================
# Runtime stage
# =============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish ./

HEALTHCHECK --interval=30s --timeout=3s --retries=3 \
  CMD curl -fsS http://127.0.0.1:8080/health || exit 1

ENTRYPOINT ["dotnet", "RentalManagementService.dll"]
