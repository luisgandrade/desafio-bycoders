#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TransactionsLog/TransactionsLog.csproj", "TransactionsLog/"]
COPY ["TransactionsLog.DataLayer.EF/TransactionsLog.DataLayer.EF.csproj", "TransactionsLog.DataLayer.EF/"]
COPY ["TransactionsLog.DataLayer.Abstractions/TransactionsLog.DataLayer.Abstractions.csproj", "TransactionsLog.DataLayer.Abstractions/"]
COPY ["TransactionsLog.Models/TransactionsLog.Models.csproj", "TransactionsLog.Models/"]
COPY ["TransactionsLog.Services/TransactionsLog.Services.csproj", "TransactionsLog.Services/"]
RUN dotnet restore "TransactionsLog/TransactionsLog.csproj"
COPY . .

FROM build AS publish
RUN dotnet publish "TransactionsLog/TransactionsLog.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TransactionsLog.dll"]