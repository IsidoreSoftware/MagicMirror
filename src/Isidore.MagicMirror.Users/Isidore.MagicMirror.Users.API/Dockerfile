FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster-arm64v8 AS build-env
WORKDIR /app

COPY src/Isidore.MagicMirror.Users/dotnet-restore.tar .
RUN tar -xvf dotnet-restore.tar
RUN dotnet restore src/Isidore.MagicMirror.Users/Isidore.MagicMirror.Users.API/Isidore.MagicMirror.Users.API.csproj && rm dotnet-restore.tar
COPY . ./
RUN ls /app/src/Isidore.MagicMirror.DAL.MongoDB
RUN dotnet publish --no-restore -c Release -o out src/Isidore.MagicMirror.Users/Isidore.MagicMirror.Users.API/Isidore.MagicMirror.Users.API.csproj

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim-arm64v8
ARG source
WORKDIR /app
EXPOSE 80
COPY --from=build-env /app/out .
RUN readlink /proc/self/exe
ENTRYPOINT ["dotnet", "Isidore.MagicMirror.Users.API.dll"]
