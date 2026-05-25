FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar solo el .csproj principal (ignora el de tests)
COPY mi-tension-backend.csproj ./
RUN dotnet restore mi-tension-backend.csproj

COPY . ./
RUN dotnet publish mi-tension-backend.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:${PORT:-8080}
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "mi-tension-backend.dll"]
