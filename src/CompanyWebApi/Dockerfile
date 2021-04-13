#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["src/CompanyWebApi/CompanyWebApi.csproj", "src/CompanyWebApi/"]
COPY ["src/CompanyWebApi.Contracts/CompanyWebApi.Contracts.csproj", "src/CompanyWebApi.Contracts/"]
COPY ["src/CompanyWebApi.Persistence/CompanyWebApi.Persistence.csproj", "src/CompanyWebApi.Persistence/"]
COPY ["src/CompanyWebApi.Core/CompanyWebApi.Core.csproj", "src/CompanyWebApi.Core/"]
RUN dotnet restore "src/CompanyWebApi/CompanyWebApi.csproj"
COPY . .
WORKDIR "/src/src/CompanyWebApi"
RUN dotnet build "CompanyWebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CompanyWebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CompanyWebApi.dll"]