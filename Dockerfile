FROM microsoft/dotnet
COPY ./src/ /app
WORKDIR /app/Isidore.MagicMirror.API

RUN ["dotnet", "migrate","project.json"]
RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]

EXPOSE 80
ENTRYPOINT ["/bin/bash"]