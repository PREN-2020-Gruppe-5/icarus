FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

#COPY *.csproj .
#RUN dotnet restore

COPY . .
WORKDIR /app
RUN dotnet publish Icarus.Sensors.Tilt -c Release -o out

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-bionic-arm64v8 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "Icarus.Sensors.Tilt.dll"]