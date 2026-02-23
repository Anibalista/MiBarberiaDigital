# Imagen base con el SDK para compilar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar todo el código
COPY . .

# Restaurar dependencias y compilar
RUN dotnet restore
RUN dotnet publish -c Release -o /app

# Imagen final más liviana solo con el runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "MiBarberiaDigital.dll"]