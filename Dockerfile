FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Todo_MinimalApi_Sample/Todo_MinimalApi_Sample.csproj", "Todo_MinimalApi_Sample/"]
RUN dotnet restore "Todo_MinimalApi_Sample/Todo_MinimalApi_Sample.csproj"
COPY . .
WORKDIR "/src/Todo_MinimalApi_Sample"
RUN dotnet build "Todo_MinimalApi_Sample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Todo_MinimalApi_Sample.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Todo_MinimalApi_Sample.dll"]
