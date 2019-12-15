FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["GrandElementApi.csproj", "GrandElementApi/"]
RUN dotnet restore "GrandElementApi.csproj"
COPY . .
WORKDIR "/src/GrandElementApi"
RUN dotnet build "GrandElementApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "GrandElementApi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
ENV TZ Asia/Yekaterinburg
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "GrandElementApi.dll"]