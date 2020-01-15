FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-alpine3.9
ARG source
WORKDIR /app
EXPOSE 80
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "Isidore.MagicMirror.API.dll"]
