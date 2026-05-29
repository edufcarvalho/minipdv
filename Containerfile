FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app --self-contained false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
COPY entrypoint.sh /app/
RUN chmod +x /app/entrypoint.sh
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["/app/entrypoint.sh"]
