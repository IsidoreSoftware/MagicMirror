find . -name '*.csproj' -o -name 'Isidore.MagicMirror.sln' -o -name 'nuget.config' \
  | sort | tar cf src/Isidore.MagicMirror.Widgets/dotnet-restore.tar -T - 2> /dev/null
docker build -f src/Isidore.MagicMirror.Widgets/Isidore.MagicMirror.Widgets.API/Dockerfile .