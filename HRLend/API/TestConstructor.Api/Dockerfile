#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8082
EXPOSE 8083

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["API/TestConstructor.Api/TestConstructor.Api.csproj", "API/TestConstructor.Api/"]
COPY ["Contracts/Authorization.Contract/Authorization.Contract.csproj", "Contracts/Authorization.Contract/"]
COPY ["Contracts/KnowledgeBase.Contract/KnowledgeBase.Contract.csproj", "Contracts/KnowledgeBase.Contract/"]
COPY ["Contracts/TestConstructor.Contract/TestConstructor.Contract.csproj", "Contracts/TestConstructor.Contract/"]
COPY ["Contracts/TestGenerator.Contract/TestGenerator.Contract.csproj", "Contracts/TestGenerator.Contract/"]
COPY ["Helpers/Helpers.csproj", "Helpers/"]
RUN dotnet restore "./API/TestConstructor.Api/./TestConstructor.Api.csproj"
COPY . .
WORKDIR "/src/API/TestConstructor.Api"
RUN dotnet build "./TestConstructor.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TestConstructor.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestConstructor.Api.dll"]