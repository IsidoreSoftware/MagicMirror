FROM microsoft/dotnet:1.1-runtime

MAINTAINER Jakub Matjanowski "kuba@matjanowski.pl"

ARG source
WORKDIR /app
EXPOSE 80
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "Isidore.MagicMirror.API.dll"]
