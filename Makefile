build-all:
	dotnet build && cd client && ng build

build-api:
	dotnet build

docker-build:
	docker build -t nilsojr/paybills-api:latest . 

docker-build-amd64:
	docker buildx build --platform=linux/amd64 -t paybills-api:latest .

docker-run:
	docker compose up -d

run:
	dotnet run --project Paybills.API/Paybills.API.csproj

test:
	dotnet test --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=lcov

coverage:
	reportgenerator -reports:"Path\To\TestProject\TestResults\{guid}\coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html