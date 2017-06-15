FROM microsoft/dotnet:1.1.1-runtime

WORKDIR /app
COPY out .

ENV ASPNETCORE_URLS http://*:5000

ENTRYPOINT dotnet ratelimit.dll