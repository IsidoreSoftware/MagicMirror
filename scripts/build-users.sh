find . -name '*.csproj' -o -name 'Isidore.MagicMirror.sln' -o -name 'nuget.config' \
  | sort | tar cf src/Isidore.MagicMirror.Users/dotnet-restore.tar -T - 2> /dev/null
docker build -f src/Isidore.MagicMirror.Users/Isidore.MagicMirror.Users.API/Dockerfile .