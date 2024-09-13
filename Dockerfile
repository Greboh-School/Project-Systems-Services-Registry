FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
EXPOSE 8080
EXPOSE 443
ARG NUGET_USERNAME
ARG NUGET_TOKEN
ARG version

COPY ["School.Project.Systems.Services.Registry/School.Project.Systems.Services.Registry.csproj", "School.Project.Systems.Services.Registry/"]
COPY ["NuGet.config", "School.Project.Systems.Services.Registry/"]

RUN dotnet restore "School.Project.Systems.Services.Registry/School.Project.Systems.Services.Registry.csproj" --configfile School.Project.Systems.Services.Registry/NuGet.config

COPY . .

RUN dotnet publish "School.Project.Systems.Services.Registry/School.Project.Systems.Services.Registry.csproj" -c Release -o out /p:Version=$version

FROM mcr.microsoft.com/dotnet/aspnet:8.0 
WORKDIR /app

COPY --from=build /src/out .
ENTRYPOINT ["dotnet", "School.Project.Systems.Services.Registry.dll"]
