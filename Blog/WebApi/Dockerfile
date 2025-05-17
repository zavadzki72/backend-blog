# Build
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS build
WORKDIR /app

COPY *.sln .
COPY WebApi/*.csproj ./WebApi/
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o out

# Run
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "WebApi.dll"]