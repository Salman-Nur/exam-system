FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["DevSkill.AWS/*.csproj", "DevSkill.AWS/"]
COPY ["ExamSystem.Application/*.csproj", "ExamSystem.Application/"]
COPY ["ExamSystem.Domain/*.csproj", "ExamSystem.Domain/"]
COPY ["ExamSystem.Infrastructure/*.csproj", "ExamSystem.Infrastructure/"]
COPY ["ExamSystem.HttpApi/*.csproj", "ExamSystem.HttpApi/"]

RUN dotnet restore "ExamSystem.HttpApi/ExamSystem.HttpApi.csproj"
COPY . .
WORKDIR "/src/ExamSystem.HttpApi"
RUN dotnet build "ExamSystem.HttpApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ExamSystem.HttpApi.csproj" -c Release -o /app

FROM build AS final
WORKDIR /app
COPY --from=publish /app .
