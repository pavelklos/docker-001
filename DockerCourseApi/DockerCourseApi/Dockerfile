FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["DockerCourseApi/DockerCourseApi.csproj", "DockerCourseApi/"]
RUN dotnet restore "DockerCourseApi/DockerCourseApi.csproj"
COPY . .
WORKDIR "/src/DockerCourseApi"
RUN dotnet publish "DockerCourseApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DockerCourseApi.dll"]
