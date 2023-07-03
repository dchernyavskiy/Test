FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 5000
EXPOSE 5001
EXPOSE 44378

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Test/Test.csproj", "Test/"]
RUN dotnet restore "Test/Test.csproj"
COPY . .
WORKDIR "/src/Test"
RUN dotnet build "Test.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Test.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Test.dll"]
