#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Example.Kafka.Api/Example.Kafka.Api.csproj", "src/Example.Kafka.Api/"]
COPY ["src/Example.Kafka.Application/Example.Kafka.Application.csproj", "src/Example.Kafka.Application/"]
COPY ["src/Example.Kafka.Domain/Example.Kafka.Domain.csproj", "src/Example.Kafka.Domain/"]
COPY ["src/Example.Kafka.Infra.Data/Example.Kafka.Infra.Data.csproj", "src/Example.Kafka.Infra.Data/"]
RUN dotnet restore "src/Example.Kafka.Api/Example.Kafka.Api.csproj"
COPY . .
WORKDIR "/src/src/Example.Kafka.Api"
RUN dotnet build "Example.Kafka.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Example.Kafka.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Example.Kafka.Api.dll"]