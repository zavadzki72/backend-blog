# Build
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS build
WORKDIR /app

COPY Blog/*.sln .
COPY Blog/WebApi/*.csproj ./Blog/WebApi/
RUN dotnet restore

COPY . .
WORKDIR /app/Blog/WebApi
RUN dotnet publish -c Release -o /app/out

# Run
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Blog/WebApi.dll"]