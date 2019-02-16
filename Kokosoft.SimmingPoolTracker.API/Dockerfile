FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
copy bin/Release/netcoreapp2.2/publish .
ENTRYPOINT ["dotnet", "Kokosoft.SimmingPoolTracker.API.dll"]
