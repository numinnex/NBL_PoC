FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["NBL_PoC/NBL_PoC_Api.csproj", "NBL_PoC/"]
RUN dotnet restore "NBL_PoC/NBL_PoC_Api.csproj"
COPY . .
WORKDIR "/src/NBL_PoC"
RUN dotnet build "NBL_PoC_Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NBL_PoC_Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NBL_PoC_Api.dll"]
