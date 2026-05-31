FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet build -c Release --no-restore
RUN dotnet publish -c Release -o /app/publish --no-build

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
COPY entrypoint.sh .
RUN chmod +x entrypoint.sh
EXPOSE 5000
ENTRYPOINT ["/bin/bash", "entrypoint.sh"]
