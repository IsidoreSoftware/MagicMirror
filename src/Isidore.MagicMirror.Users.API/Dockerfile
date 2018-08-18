FROM microsoft/dotnet:2.1-runtime
WORKDIR /dotnetapp
COPY bin/Docker .
ENV ASPNERCORE_ENVIRONMENT Development
ENTRYPOINT ["dotnet", "Isidore.MagicMirror.Users.API.dll"]
