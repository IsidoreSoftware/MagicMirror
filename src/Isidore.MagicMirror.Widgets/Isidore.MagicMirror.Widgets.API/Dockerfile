FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster-arm64v8 AS build-env
WORKDIR /app

COPY . ./
RUN ls
RUN dotnet restore Isidore.MagicMirror.Widgets/Isidore.MagicMirror.Widgets.API/Isidore.MagicMirror.Widgets.API.csproj
RUN dotnet publish -c Release -o out Isidore.MagicMirror.Widgets/Isidore.MagicMirror.Widgets.API/Isidore.MagicMirror.Widgets.API.csproj

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim-arm64v8
ARG source
WORKDIR /app
EXPOSE 80
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Isidore.MagicMirror.Widgets.API.dll"]
