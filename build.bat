dotnet restore src/monster
dotnet build src/monster

dotnet restore tests/monster.Tests
dotnet build tests/monster.Tests
dotnet test tests/monster.Tests
