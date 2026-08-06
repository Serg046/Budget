FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
COPY Budget.Server/Budget.Server.csproj Budget.Server/Budget.Server.csproj
COPY Budget.Client/Budget.Client.csproj Budget.Client/Budget.Client.csproj
RUN dotnet restore Budget.Server/Budget.Server.csproj && dotnet restore Budget.Client/Budget.Client.csproj
COPY . .
RUN --mount=type=secret,id=ENABLEBANKING_PRIVATE_KEY \
    --mount=type=secret,id=AUTH \
    sed -i \
      -e "s|\"ConnectionString\": \"mongodb://localhost:27017\"|\"ConnectionString\": \"mongodb://budget-mongo-service:27017\"|" \
      -e "s|\"PrivateKey\": \"\"|\"PrivateKey\": \"$(cat /run/secrets/ENABLEBANKING_PRIVATE_KEY)\"|" \
      -e "s|\"Users\": \[\]|\"Users\": $(cat /run/secrets/AUTH)|" \
      Budget.Server/appsettings.json
RUN dotnet publish Budget.Server/Budget.Server.csproj

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /Budget.Server/bin/Release/net10.0/publish/ .
EXPOSE 8080
ENTRYPOINT ["dotnet", "./Budget.Server.dll"]
