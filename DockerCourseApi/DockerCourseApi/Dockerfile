FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["DockerCourseApi/DockerCourseApi.csproj", "DockerCourseApi/"]
RUN dotnet restore "DockerCourseApi/DockerCourseApi.csproj"
COPY . .
# Without tests
WORKDIR "/src/DockerCourseApi"
RUN dotnet publish "DockerCourseApi.csproj" -c Release -o /app/publish /p:UseAppHost=false
# If you want to include tests, uncomment the following lines and comment the above two lines
# WORKDIR "/src"
# RUN dotnet build "DockerCourseApi/DockerCourseApi.csproj" -c Release -o /app/build
# RUN dotnet test "DockerCourseApi.Tests/DockerCourseApi.Tests.csproj"
# RUN dotnet publish "DockerCourseApi/DockerCourseApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

EXPOSE 80
EXPOSE 443
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DockerCourseApi.dll"]
