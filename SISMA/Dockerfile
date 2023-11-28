#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM giti.is-bg.net:5050/docker-images/dotnetcore/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM giti.is-bg.net:5050/docker-images/dotnetcore/sdk:6.0 AS build
WORKDIR /src
COPY ["NuGet.config", "NuGet.config"]
COPY ["SISMA/SISMA.csproj", "SISMA/"]
COPY ["SISMA.Core/SISMA.Core.csproj", "SISMA.Core/"]
COPY ["SISMA.Infrastructure/SISMA.Infrastructure.csproj", "SISMA.Infrastructure/"]
RUN dotnet restore "SISMA/SISMA.csproj"
COPY . .
WORKDIR "/src/SISMA"
RUN dotnet build "SISMA.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SISMA.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SISMA.dll"]
