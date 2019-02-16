FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["../Kokosoft.SwimmingPoolTracker.ImportSchedule/Kokosoft.SwimmingPoolTracker.ImportSchedule.csproj", "../Kokosoft.SwimmingPoolTracker.ImportSchedule/"]
RUN dotnet restore "../Kokosoft.SwimmingPoolTracker.ImportSchedule/Kokosoft.SwimmingPoolTracker.ImportSchedule.csproj"
COPY . .
WORKDIR "/src/../Kokosoft.SwimmingPoolTracker.ImportSchedule"
RUN dotnet build "Kokosoft.SwimmingPoolTracker.ImportSchedule.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Kokosoft.SwimmingPoolTracker.ImportSchedule.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Kokosoft.SwimmingPoolTracker.ImportSchedule.dll"]