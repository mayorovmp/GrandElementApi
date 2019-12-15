FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["OurGardenAPI/OurGardenAPI.csproj", "OurGardenAPI/"]
RUN dotnet restore "OurGardenAPI/OurGardenAPI.csproj"
COPY . .
WORKDIR "/src/OurGardenAPI"
RUN dotnet build "OurGardenAPI.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "OurGardenAPI.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
ENV TZ Asia/Yekaterinburg
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "OurGardenAPI.dll"]