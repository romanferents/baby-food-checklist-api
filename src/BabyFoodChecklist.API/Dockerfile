FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/BabyFoodChecklist.API/BabyFoodChecklist.API.csproj", "src/BabyFoodChecklist.API/"]
COPY ["src/BabyFoodChecklist.Application/BabyFoodChecklist.Application.csproj", "src/BabyFoodChecklist.Application/"]
COPY ["src/BabyFoodChecklist.Infrastructure/BabyFoodChecklist.Infrastructure.csproj", "src/BabyFoodChecklist.Infrastructure/"]
COPY ["src/BabyFoodChecklist.Domain/BabyFoodChecklist.Domain.csproj", "src/BabyFoodChecklist.Domain/"]
RUN dotnet restore "src/BabyFoodChecklist.API/BabyFoodChecklist.API.csproj"
COPY . .
WORKDIR "/src/src/BabyFoodChecklist.API"
RUN dotnet build "BabyFoodChecklist.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BabyFoodChecklist.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BabyFoodChecklist.API.dll"]
