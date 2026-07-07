FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Final Project.csproj", "."]
RUN dotnet restore "./Final Project.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Final Project.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Final Project.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Final Project.dll"]