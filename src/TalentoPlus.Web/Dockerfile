# Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/TalentoPlus.Web/TalentoPlus.Web.csproj", "src/TalentoPlus.Web/"]
COPY ["src/TalentoPlus.Application/TalentoPlus.Application.csproj", "src/TalentoPlus.Application/"]
COPY ["src/TalentoPlus.Domain/TalentoPlus.Domain.csproj", "src/TalentoPlus.Domain/"]
COPY ["src/TalentoPlus.Infrastructure/TalentoPlus.Infrastructure.csproj", "src/TalentoPlus.Infrastructure/"]
RUN dotnet restore "src/TalentoPlus.Web/TalentoPlus.Web.csproj"
COPY . .
WORKDIR "/src/src/TalentoPlus.Web"
RUN dotnet build "TalentoPlus.Web.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "TalentoPlus.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TalentoPlus.Web.dll"]
