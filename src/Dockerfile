#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Debugger support for kubernetes
RUN apt-get update && apt-get -y install wget
RUN wget https://aka.ms/getvsdbgsh -O - 2>/dev/null | /bin/sh /dev/stdin -v latest -l /root/vsdbg

# Build from /src dir and copy files in baseline folder
WORKDIR /src
COPY . .

# Restore project and set working dir for following build
RUN dotnet restore

# Build and then publish
ARG Mode=Debug
RUN dotnet build "actortest.csproj" -c $Mode -o /app/build

FROM build AS publish
ARG Mode=Debug
RUN dotnet publish "actortest.csproj" -c $Mode -o /app/publish

FROM base AS final

# Ensure we have debug support - pull from build image
WORKDIR /root/vsdbg
COPY --from=build /root/vsdbg .
RUN apt-get update && apt-get -y install procps

# Copy published app
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "actortest.dll"]
