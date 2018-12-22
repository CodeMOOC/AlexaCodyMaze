# ---
# First stage (build)
FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app

# Copy solution as distinct layer
COPY *.sln .
COPY Bot/*.csproj ./Bot/
RUN dotnet restore

# Copy everything else and build
COPY Bot/. ./Bot/
WORKDIR /app/Bot
RUN dotnet publish -c Release -o out

# ---
# Second stage (execution)
FROM microsoft/dotnet:2.1-aspnetcore-runtime AS runtime
WORKDIR /app
COPY --from=build /app/Bot/out ./

# Run on localhost:8779
ENV ASPNETCORE_URLS http://+:5001
EXPOSE 5001

# Drop privileges
USER 1000

ENTRYPOINT ["dotnet", "Bot.dll"]
