# Estágio 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia TODOS os .csproj da solução inteira
COPY MyBancoApi.sln .
COPY src/MyBancoApi.ContaCorrente.Api/MyBancoApi.ContaCorrente.Api.csproj src/MyBancoApi.ContaCorrente.Api/
COPY src/MyBancoApi.ContaCorrente.Application/MyBancoApi.ContaCorrente.Application.csproj src/MyBancoApi.ContaCorrente.Application/
COPY src/MyBancoApi.ContaCorrente.Domain/MyBancoApi.ContaCorrente.Domain.csproj src/MyBancoApi.ContaCorrente.Domain/
COPY src/MyBancoApi.ContaCorrente.Infrastructure/MyBancoApi.ContaCorrente.Infrastructure.csproj src/MyBancoApi.ContaCorrente.Infrastructure/

COPY src/MyBancoApi.Transferencia.Api/MyBancoApi.Transferencia.Api.csproj src/MyBancoApi.Transferencia.Api/
COPY src/MyBancoApi.Transferencia.Application/MyBancoApi.Transferencia.Application.csproj src/MyBancoApi.Transferencia.Application/
COPY src/MyBancoApi.Transferencia.Domain/MyBancoApi.Transferencia.Domain.csproj src/MyBancoApi.Transferencia.Domain/
COPY src/MyBancoApi.Transferencia.Infrastructure/MyBancoApi.Transferencia.Infrastructure.csproj src/MyBancoApi.Transferencia.Infrastructure/

COPY src/MyBancoApi.Tarifas.Worker/MyBancoApi.Tarifas.Worker.csproj src/MyBancoApi.Tarifas.Worker/
COPY src/MyBancoApi.Tarifas.Application/MyBancoApi.Tarifas.Application.csproj src/MyBancoApi.Tarifas.Application/
COPY src/MyBancoApi.Tarifas.Domain/MyBancoApi.Tarifas.Domain.csproj src/MyBancoApi.Tarifas.Domain/
COPY src/MyBancoApi.Tarifas.Infrastructure/MyBancoApi.Tarifas.Infrastructure.csproj src/MyBancoApi.Tarifas.Infrastructure/

# Restaura todos os pacotes da solução
RUN dotnet restore MyBancoApi.sln

# Copia o restante do código-fonte e publica a API
COPY . .
WORKDIR "/src/src/MyBancoApi.ContaCorrente.Api"
RUN dotnet publish -c Release -o /app/publish

# Estágio 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "MyBancoApi.ContaCorrente.Api.dll"]