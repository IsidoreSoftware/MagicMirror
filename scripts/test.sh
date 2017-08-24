for D in `find ./tests/ -type f -name *Test[s].csproj`
do
    dotnet test --no-build "${D}"
done