FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["SoftwarePackageManager/SoftwarePackageManager.csproj", "SoftwarePackageManager/"]
RUN dotnet restore "SoftwarePackageManager/SoftwarePackageManager.csproj"
COPY . .
WORKDIR "/src/SoftwarePackageManager"
RUN dotnet build "SoftwarePackageManager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SoftwarePackageManager.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SoftwarePackageManager.dll"]