FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
COPY SpendPulse.Server/SpendPulse.Server.csproj SpendPulse.Server/SpendPulse.Server.csproj
COPY SpendPulse.Client/SpendPulse.Client.csproj SpendPulse.Client/SpendPulse.Client.csproj
RUN dotnet restore SpendPulse.Server/SpendPulse.Server.csproj && dotnet restore SpendPulse.Client/SpendPulse.Client.csproj
COPY . .
RUN --mount=type=secret,id=ENABLEBANKING_PRIVATE_KEY \
    --mount=type=secret,id=AUTH \
    sed -i \
      -e "s|\"ConnectionString\": \"mongodb://localhost:27017\"|\"ConnectionString\": \"mongodb://mongo-service:27017\"|" \
      -e "s|\"PrivateKey\": \"\"|\"PrivateKey\": \"$(cat /run/secrets/ENABLEBANKING_PRIVATE_KEY)\"|" \
      -e "s|\"Users\": \[\]|\"Users\": $(cat /run/secrets/AUTH)|" \
      SpendPulse.Server/appsettings.json
RUN dotnet publish SpendPulse.Server/SpendPulse.Server.csproj

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /SpendPulse.Server/bin/Release/net10.0/publish/ .
EXPOSE 8080
ENTRYPOINT ["dotnet", "./SpendPulse.Server.dll"]
