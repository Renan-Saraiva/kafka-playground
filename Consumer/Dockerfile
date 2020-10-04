FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

RUN dotnet publish "Consumer.csproj" -c Release -o out
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env

COPY . ./
RUN dotnet restore "Consumer.csproj"
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Consumer.dll"]