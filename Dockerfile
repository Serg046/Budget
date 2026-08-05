FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG TARGETARCH
COPY Budget.Server/Budget.Server.csproj Budget.Server/Budget.Server.csproj
COPY Budget.Client/Budget.Client.csproj Budget.Client/Budget.Client.csproj
RUN dotnet restore Budget.Server/Budget.Server.csproj -a $TARGETARCH && dotnet restore Budget.Client/Budget.Client.csproj -a $TARGETARCH
COPY . .
RUN dotnet publish Budget.Server/Budget.Server.csproj -a $TARGETARCH --no-restore

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /Budget.Server/bin/Release/net10.0/*/publish/ .
EXPOSE 8080
ENTRYPOINT ["dotnet", "./Budget.Server.dll"]
