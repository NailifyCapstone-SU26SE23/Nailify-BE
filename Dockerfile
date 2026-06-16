# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Nailify.Capstone.Presentation/Nailify.Capstone.Presentation.csproj", "Nailify.Capstone.Presentation/"]
COPY ["Nailify.Capstone.Application/Nailify.Capstone.Application.csproj", "Nailify.Capstone.Application/"]
COPY ["Nailify.Capstone.Domain/Nailify.Capstone.Domain.csproj", "Nailify.Capstone.Domain/"]
COPY ["Nailify.Capstone.Infrastructure/Nailify.Capstone.Infrastructure.csproj", "Nailify.Capstone.Infrastructure/"]

RUN dotnet restore "Nailify.Capstone.Presentation/Nailify.Capstone.Presentation.csproj"

COPY . .

WORKDIR /src/Nailify.Capstone.Presentation
RUN dotnet publish "Nailify.Capstone.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

RUN mkdir -p /app/image

ENTRYPOINT ["dotnet", "Nailify.Capstone.Presentation.dll"]