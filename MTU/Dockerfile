FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os .csproj (caminhos relativos à raiz da solução)
COPY ["MTU/MTU.csproj", "MTU/"]
COPY ["MTU.Tests/MTU.Tests.csproj", "MTU.Tests/"]

# Restaura pacotes do projeto principal
RUN dotnet restore "MTU/MTU.csproj"

# Copia TODO o código (incluindo testes, mas eles não serão usados)
COPY . .

# Build apenas do projeto principal
RUN dotnet build "MTU/MTU.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MTU/MTU.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MTU.dll"]