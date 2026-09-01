FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ECommerceMVC/ECommerceMVC.csproj", "ECommerceMVC/"]
RUN dotnet restore "ECommerceMVC/ECommerceMVC.csproj"

COPY . .
WORKDIR "/src/ECommerceMVC"

RUN dotnet publish "ECommerceMVC.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "ECommerceMVC.dll"]