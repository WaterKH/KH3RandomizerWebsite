#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["KH3Randomizer/KH3Randomizer.csproj", "KH3Randomizer/"]
RUN dotnet restore "KH3Randomizer/KH3Randomizer.csproj"
COPY . .
WORKDIR "/src/KH3Randomizer"
RUN dotnet build "KH3Randomizer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KH3Randomizer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KH3Randomizer.dll"]