FROM microsoft/aspnetcore:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:3.1 AS build
WORKDIR /src
COPY *.sln ./
COPY ReportService/ReportService.csproj ReportService/
RUN dotnet restore
COPY . .
WORKDIR /src/ReportService
RUN dotnet build -c Release_PostgreSql -o /app

FROM build AS publish
RUN dotnet publish -c Release_PostgreSql -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ReportService.dll"]
